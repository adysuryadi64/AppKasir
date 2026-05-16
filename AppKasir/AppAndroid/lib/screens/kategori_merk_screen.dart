import 'package:flutter/material.dart';
import '../services/api_service.dart';

/// Screen CRUD Kategori & Merk
/// [initialTab]: 'kategori' atau 'merk'
class KategoriMerkScreen extends StatefulWidget {
  final String initialTab;
  const KategoriMerkScreen({super.key, this.initialTab = 'kategori'});

  @override
  State<KategoriMerkScreen> createState() => _KategoriMerkScreenState();
}

class _KategoriMerkScreenState extends State<KategoriMerkScreen>
    with SingleTickerProviderStateMixin {
  static const _green = Color(0xFF16A34A);

  late TabController _tabCtrl;
  List<Map<String, dynamic>> _kategori = [];
  List<Map<String, dynamic>> _merk = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _tabCtrl = TabController(
      length: 2,
      vsync: this,
      initialIndex: widget.initialTab == 'merk' ? 1 : 0,
    );
    _loadAll();
  }

  @override
  void dispose() {
    _tabCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadAll() async {
    setState(() => _isLoading = true);
    try {
      final r1 = await ApiService.getKategori();
      final r2 = await ApiService.getMerk();
      if (!mounted) return;
      setState(() {
        if (r1['status'] == 'success') {
          _kategori = List<Map<String, dynamic>>.from(r1['data'] ?? []);
          debugPrint('[KategoriMerk] ✅ kategori: ${_kategori.length}');
        } else {
          debugPrint('[KategoriMerk] ⚠️ getKategori: ${r1['message']}');
        }
        if (r2['status'] == 'success') {
          _merk = List<Map<String, dynamic>>.from(r2['data'] ?? []);
          debugPrint('[KategoriMerk] ✅ merk: ${_merk.length}');
        } else {
          debugPrint('[KategoriMerk] ⚠️ getMerk: ${r2['message']}');
        }
      });
    } catch (e) {
      debugPrint('[KategoriMerk] ❌ _loadAll error: $e');
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  // ── Dialog tambah/edit ────────────────────────────────────────
  Future<void> _showDialog({
    required String type,
    Map<String, dynamic>? existing,
  }) async {
    final isEdit = existing != null;
    final kodeCtrl = TextEditingController(
      text: existing?['kode']?.toString() ?? '',
    );
    final namaCtrl = TextEditingController(
      text: existing?['nama']?.toString() ?? '',
    );
    // kategori: field ini adalah 'jenis', default 'Barang'
    // merk: field ini adalah 'keterangan'
    final ketCtrl = TextEditingController(
      text: existing != null
          ? (type == 'kategori'
                ? existing['jenis']?.toString() ?? 'Barang'
                : existing['keterangan']?.toString() ?? '')
          : (type == 'kategori' ? 'Barang' : ''),
    );

    // Generate kode otomatis saat tambah baru
    if (!isEdit) {
      try {
        final kode = type == 'kategori'
            ? await ApiService.generateKodeKategori()
            : await ApiService.generateKodeMerk();
        kodeCtrl.text = kode;
      } catch (e) {
        debugPrint('[KategoriMerk] ❌ generateKode error: $e');
      }
    }

    if (!mounted) return;

    await showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Text(
          '${isEdit ? 'Edit' : 'Tambah'} ${type == 'kategori' ? 'Kategori' : 'Merk'}',
          style: const TextStyle(fontSize: 16),
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Kode — selalu tampil, readonly
            TextField(
              controller: kodeCtrl,
              readOnly: true,
              decoration: InputDecoration(
                labelText: 'Kode',
                filled: true,
                fillColor: Colors.grey.shade100,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
            ),
            const SizedBox(height: 10),
            TextField(
              controller: namaCtrl,
              autofocus: true,
              decoration: InputDecoration(
                labelText: 'Nama',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
            ),
            const SizedBox(height: 10),
            TextField(
              controller: ketCtrl,
              decoration: InputDecoration(
                labelText: type == 'kategori'
                    ? 'Jenis'
                    : 'Keterangan (opsional)',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Batal'),
          ),
          ElevatedButton(
            onPressed: () async {
              Navigator.of(ctx).pop();
              try {
                Map<String, dynamic> result;
                if (type == 'kategori') {
                  if (isEdit) {
                    result = await ApiService.updateKategori(
                      existing['kode'].toString(),
                      namaCtrl.text,
                      ketCtrl.text.isEmpty ? 'Barang' : ketCtrl.text,
                    );
                  } else {
                    // Cek kode duplikat → generate ulang sebelum kirim
                    String kode = kodeCtrl.text;
                    final existing2 = _kategori.any(
                      (k) =>
                          k['kode']?.toString().toUpperCase() ==
                          kode.toUpperCase(),
                    );
                    if (existing2) {
                      kode = await ApiService.generateKodeKategori();
                    }
                    result = await ApiService.addKategori(
                      kode,
                      namaCtrl.text,
                      ketCtrl.text.isEmpty ? 'Barang' : ketCtrl.text,
                    );
                  }
                } else {
                  if (isEdit) {
                    result = await ApiService.updateMerk(
                      existing['kode'].toString(),
                      namaCtrl.text,
                      ketCtrl.text,
                    );
                  } else {
                    // Cek kode duplikat → generate ulang sebelum kirim
                    String kode = kodeCtrl.text;
                    final existing2 = _merk.any(
                      (k) =>
                          k['kode']?.toString().toUpperCase() ==
                          kode.toUpperCase(),
                    );
                    if (existing2) {
                      kode = await ApiService.generateKodeMerk();
                    }
                    result = await ApiService.addMerk(
                      kode,
                      namaCtrl.text,
                      ketCtrl.text,
                    );
                  }
                }

                if (result['status'] == 'error') {
                  if (mounted) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      SnackBar(
                        content: Text(
                          result['message']?.toString() ??
                              'Gagal menyimpan data',
                        ),
                        backgroundColor: Colors.red,
                      ),
                    );
                  }
                  return;
                }
                _loadAll();
              } catch (e) {
                if (mounted) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Gagal: $e'),
                      backgroundColor: Colors.red,
                    ),
                  );
                }
              }
            },
            child: Text(isEdit ? 'Simpan' : 'Tambah'),
          ),
        ],
      ),
    );
  }

  Future<void> _delete(String type, String kode, String nama) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Hapus'),
        content: Text('Hapus "$nama"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Batal'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text('Hapus'),
          ),
        ],
      ),
    );
    if (ok != true) return;
    try {
      if (type == 'kategori') {
        await ApiService.deleteKategori(kode);
      } else {
        await ApiService.deleteMerk(kode);
      }
      _loadAll();
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Gagal hapus: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Kategori & Merk'),
        bottom: TabBar(
          controller: _tabCtrl,
          indicatorColor: Colors.white,
          labelColor: Colors.white,
          unselectedLabelColor: Colors.white70,
          tabs: const [
            Tab(text: 'Kategori'),
            Tab(text: 'Merk'),
          ],
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _green))
          : TabBarView(
              controller: _tabCtrl,
              children: [
                _buildList('kategori', _kategori),
                _buildList('merk', _merk),
              ],
            ),
      floatingActionButton: FloatingActionButton(
        onPressed: () =>
            _showDialog(type: _tabCtrl.index == 0 ? 'kategori' : 'merk'),
        child: const Icon(Icons.add),
      ),
    );
  }

  Widget _buildList(String type, List<Map<String, dynamic>> data) {
    if (data.isEmpty) {
      return Center(
        child: Text(
          'Belum ada data',
          style: TextStyle(color: Colors.grey.shade500),
        ),
      );
    }
    return RefreshIndicator(
      onRefresh: _loadAll,
      color: _green,
      child: ListView.separated(
        padding: const EdgeInsets.fromLTRB(14, 14, 14, 80),
        itemCount: data.length,
        separatorBuilder: (ctx2, idx) => const SizedBox(height: 6),
        itemBuilder: (_, i) {
          final item = data[i];
          final kode = item['kode']?.toString() ?? '';
          final nama = item['nama']?.toString() ?? '';
          final ket = item['keterangan']?.toString() ?? '';

          return Container(
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.grey.shade200),
            ),
            child: ListTile(
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 14,
                vertical: 4,
              ),
              leading: Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: _green.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Center(
                  child: Text(
                    kode.isNotEmpty ? kode[0].toUpperCase() : '?',
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: _green,
                    ),
                  ),
                ),
              ),
              title: Text(
                nama,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w600,
                ),
              ),
              subtitle: ket.isNotEmpty
                  ? Text(
                      ket,
                      style: TextStyle(
                        fontSize: 11,
                        color: Colors.grey.shade500,
                      ),
                    )
                  : Text(
                      kode,
                      style: TextStyle(
                        fontSize: 11,
                        color: Colors.grey.shade400,
                      ),
                    ),
              trailing: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    icon: Icon(
                      Icons.edit_outlined,
                      size: 18,
                      color: Colors.blue.shade600,
                    ),
                    onPressed: () => _showDialog(type: type, existing: item),
                    tooltip: 'Edit',
                  ),
                  IconButton(
                    icon: Icon(
                      Icons.delete_outline,
                      size: 18,
                      color: Colors.red.shade400,
                    ),
                    onPressed: () => _delete(type, kode, nama),
                    tooltip: 'Hapus',
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
