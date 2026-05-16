import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import '../../providers/auth_provider.dart';
import '../../providers/penjualan_provider.dart';
import '../../services/api_service.dart';

class Step3PaymentScreen extends StatefulWidget {
  final VoidCallback onNext;
  final VoidCallback onBack;
  const Step3PaymentScreen({
    super.key,
    required this.onNext,
    required this.onBack,
  });

  @override
  State<Step3PaymentScreen> createState() => _Step3PaymentScreenState();
}

class _Step3PaymentScreenState extends State<Step3PaymentScreen> {
  static const _green = Color(0xFF16A34A);
  final _fmt = NumberFormat('#,##0.##', 'id_ID');

  // Controller diskon global
  final _diskonPersenCtrl = TextEditingController();
  final _diskonRpCtrl = TextEditingController();
  // Controller pajak
  final _pajakPersenCtrl = TextEditingController();
  final _pajakRpCtrl = TextEditingController();
  // Controller biaya kirim
  final _kirimCtrl = TextEditingController();
  // Controller pembayaran
  final _tunaiCtrl = TextEditingController();
  final _transferCtrl = TextEditingController();
  // Controller info bank
  final _bankCtrl = TextEditingController();
  final _noRekCtrl = TextEditingController();
  final _namaRekCtrl = TextEditingController();
  final _noRefCtrl = TextEditingController();

  List<Map<String, dynamic>> _akunKasList = [];
  List<Map<String, dynamic>> _akunTransferList = [];
  bool _loadingAkun = false;

  // Flag anti-loop untuk sinkronisasi % ↔ Rp
  bool _updatingDiskon = false;
  bool _updatingPajak = false;

  @override
  void initState() {
    super.initState();
    _loadAkun();
    WidgetsBinding.instance.addPostFrameCallback((_) => _syncFromProvider());
  }

  @override
  void dispose() {
    for (final c in [
      _diskonPersenCtrl,
      _diskonRpCtrl,
      _pajakPersenCtrl,
      _pajakRpCtrl,
      _kirimCtrl,
      _tunaiCtrl,
      _transferCtrl,
      _bankCtrl,
      _noRekCtrl,
      _namaRekCtrl,
      _noRefCtrl,
    ]) {
      c.dispose();
    }
    super.dispose();
  }

  void _syncFromProvider() {
    final prov = context.read<PenjualanProvider>();
    if (prov.diskonPersen > 0) {
      _diskonPersenCtrl.text = prov.diskonPersen.toStringAsFixed(0);
    }
    if (prov.diskonRp > 0) {
      _diskonRpCtrl.text = prov.diskonRp.toStringAsFixed(0);
    }
    if (prov.pajakPersen > 0) {
      _pajakPersenCtrl.text = prov.pajakPersen.toStringAsFixed(0);
    }
    if (prov.pajakRp > 0) {
      _pajakRpCtrl.text = prov.pajakRp.toStringAsFixed(0);
    }
    if (prov.biayaKirim > 0) {
      _kirimCtrl.text = prov.biayaKirim.toStringAsFixed(0);
    }
    if (prov.nominalTunai > 0) {
      _tunaiCtrl.text = prov.nominalTunai.toStringAsFixed(0);
    }
    if (prov.nominalTransfer > 0) {
      _transferCtrl.text = prov.nominalTransfer.toStringAsFixed(0);
    }
    _bankCtrl.text = prov.bank;
    _noRekCtrl.text = prov.noRek;
    _namaRekCtrl.text = prov.namaRek;
    _noRefCtrl.text = prov.noRef;
  }

  Future<void> _loadAkun() async {
    setState(() => _loadingAkun = true);
    try {
      final results = await Future.wait([
        ApiService.getAkunCOA(
          tipe: 'KAS',
        ).catchError((_) => <String, dynamic>{}),
        ApiService.getAkunCOA(
          tipe: 'BANK',
        ).catchError((_) => <String, dynamic>{}),
      ]);
      if (!mounted) return;
      setState(() {
        if (results[0]['status'] == 'success') {
          _akunKasList = List<Map<String, dynamic>>.from(
            results[0]['data'] ?? [],
          );
        }
        if (results[1]['status'] == 'success') {
          _akunTransferList = List<Map<String, dynamic>>.from(
            results[1]['data'] ?? [],
          );
        }
      });
      debugPrint(
        '[Step3] akun KAS: ${_akunKasList.length} | BANK: ${_akunTransferList.length}',
      );

      final prov = context.read<PenjualanProvider>();
      final auth = context.read<AuthProvider>();
      final perusahaan = auth.perusahaan;
      final lokasi = auth.selectedLocation ?? 'TOKO';

      if (prov.akunKas == null) {
        if (perusahaan != null) {
          final defKas = perusahaan.akunKasUntukLokasi(lokasi);
          debugPrint('[Step3] defKas kode=${defKas.kode}');
          if (defKas.kode.isNotEmpty) {
            final found = _akunKasList.firstWhere(
              (a) => a['KODE_AKUN']?.toString() == defKas.kode,
              orElse: () => {
                'KODE_AKUN': defKas.kode,
                'NAMA_AKUN': defKas.nama,
              },
            );
            prov.setAkunKas(found);
          } else if (_akunKasList.isNotEmpty) {
            prov.setAkunKas(_akunKasList.first);
          }
        } else if (_akunKasList.isNotEmpty) {
          prov.setAkunKas(_akunKasList.first);
        }
      }
      if (prov.akunTransfer == null) {
        if (perusahaan != null) {
          final defTrf = perusahaan.akunTransfer;
          debugPrint('[Step3] defTrf kode=${defTrf.kode}');
          if (defTrf.kode.isNotEmpty) {
            final found = _akunTransferList.firstWhere(
              (a) => a['KODE_AKUN']?.toString() == defTrf.kode,
              orElse: () => {
                'KODE_AKUN': defTrf.kode,
                'NAMA_AKUN': defTrf.nama,
              },
            );
            prov.setAkunTransfer(found);
          } else if (_akunTransferList.isNotEmpty) {
            prov.setAkunTransfer(_akunTransferList.first);
          }
        } else if (_akunTransferList.isNotEmpty) {
          prov.setAkunTransfer(_akunTransferList.first);
        }
      }
    } catch (e) {
      debugPrint('[Step3] _loadAkun error: $e');
    } finally {
      if (mounted) setState(() => _loadingAkun = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final prov = context.watch<PenjualanProvider>();

    return SingleChildScrollView(
      padding: const EdgeInsets.fromLTRB(16, 16, 16, 100),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // ── Ringkasan item ──────────────────────────────────────
          _ringkasanItem(prov),
          const SizedBox(height: 16),

          // ── Diskon · Pajak · Kirim ──────────────────────────────
          Container(
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: Colors.grey.shade200),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Diskon — isi % → Rp otomatis, isi Rp → % otomatis
                _inlineNumRow(
                  icon: Icons.discount_outlined,
                  label: 'Diskon',
                  ctrlPersen: _diskonPersenCtrl,
                  ctrlRp: _diskonRpCtrl,
                  onChangedPersen: (v) {
                    if (_updatingDiskon) return;
                    _updatingDiskon = true;
                    final d = double.tryParse(v) ?? 0;
                    prov.setDiskonPersen(d);
                    // Isi Rp otomatis dari %
                    final rp = prov.diskonGlobalRp;
                    _diskonRpCtrl.text = rp > 0 ? rp.toStringAsFixed(0) : '';
                    _updatingDiskon = false;
                  },
                  onChangedRp: (v) {
                    if (_updatingDiskon) return;
                    _updatingDiskon = true;
                    final d = double.tryParse(v) ?? 0;
                    prov.setDiskonRp(d);
                    // Isi % otomatis dari Rp
                    final persen = prov.diskonPersen;
                    _diskonPersenCtrl.text = persen > 0
                        ? persen
                              .toStringAsFixed(2)
                              .replaceAll(RegExp(r'\.?0+$'), '')
                        : '';
                    _updatingDiskon = false;
                  },
                  hintPersen: 'Disc %',
                  hintRp: 'Disc Rp',
                  resultLabel: prov.diskonGlobalRp > 0
                      ? '- Rp ${_fmt.format(prov.diskonGlobalRp)}'
                      : null,
                  resultColor: Colors.red,
                ),
                const SizedBox(height: 10),
                const Divider(height: 1),
                const SizedBox(height: 10),
                // Pajak — dihitung dari (subtotal - diskon), isi % → Rp otomatis
                _inlineNumRow(
                  icon: Icons.receipt_outlined,
                  label: 'Pajak',
                  ctrlPersen: _pajakPersenCtrl,
                  ctrlRp: _pajakRpCtrl,
                  onChangedPersen: (v) {
                    if (_updatingPajak) return;
                    _updatingPajak = true;
                    final d = double.tryParse(v) ?? 0;
                    prov.setPajakPersen(d);
                    final rp = prov.pajakNominal;
                    _pajakRpCtrl.text = rp > 0 ? rp.toStringAsFixed(0) : '';
                    _updatingPajak = false;
                  },
                  onChangedRp: (v) {
                    if (_updatingPajak) return;
                    _updatingPajak = true;
                    final d = double.tryParse(v) ?? 0;
                    prov.setPajakRp(d);
                    final persen = prov.pajakPersen;
                    _pajakPersenCtrl.text = persen > 0
                        ? persen
                              .toStringAsFixed(2)
                              .replaceAll(RegExp(r'\.?0+$'), '')
                        : '';
                    _updatingPajak = false;
                  },
                  hintPersen: 'Pajak %',
                  hintRp: 'Pajak Rp',
                  resultLabel: prov.pajakNominal > 0
                      ? '+ Rp ${_fmt.format(prov.pajakNominal)}'
                      : null,
                  resultColor: Colors.orange,
                ),
                const SizedBox(height: 10),
                const Divider(height: 1),
                const SizedBox(height: 10),
                // Biaya kirim
                Row(
                  children: [
                    Icon(
                      Icons.local_shipping_outlined,
                      size: 15,
                      color: _green,
                    ),
                    const SizedBox(width: 6),
                    const SizedBox(
                      width: 64,
                      child: Text(
                        'Kirim',
                        style: TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w600,
                          color: Color(0xFF1E293B),
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: _numFieldCompact(
                        'Biaya kirim',
                        _kirimCtrl,
                        onDone: (v) =>
                            prov.setBiayaKirim(double.tryParse(v) ?? 0),
                        onChanged: (v) =>
                            prov.setBiayaKirim(double.tryParse(v) ?? 0),
                      ),
                    ),
                    const SizedBox(width: 88),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 10),

          // ── Grand total ─────────────────────────────────────────
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 16),
            decoration: BoxDecoration(
              color: _green,
              borderRadius: BorderRadius.circular(14),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Grand Total',
                  style: TextStyle(fontSize: 14, color: Colors.white70),
                ),
                Text(
                  'Rp ${_fmt.format(prov.grandTotal)}',
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),

          // ── Pembayaran Tunai ────────────────────────────────────
          _sectionCard('Pembayaran Tunai', Icons.payments_outlined, [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  flex: 3,
                  child: _loadingAkun
                      ? const Center(
                          child: Padding(
                            padding: EdgeInsets.all(8),
                            child: CircularProgressIndicator(strokeWidth: 2),
                          ),
                        )
                      : _akunDropdown(
                          'Akun Kas',
                          _akunKasList,
                          prov.akunKas,
                          prov.setAkunKas,
                        ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  flex: 2,
                  child: _numField(
                    '',
                    _tunaiCtrl,
                    hint: 'Nominal tunai',
                    onDone: (v) =>
                        prov.setNominalTunai(double.tryParse(v) ?? 0),
                  ),
                ),
              ],
            ),
          ]),
          const SizedBox(height: 10),

          // ── Pembayaran Transfer ─────────────────────────────────
          _sectionCard('Pembayaran Transfer', Icons.account_balance_outlined, [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  flex: 3,
                  child: _akunDropdown(
                    'Akun Bank',
                    _akunTransferList,
                    prov.akunTransfer,
                    prov.setAkunTransfer,
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  flex: 2,
                  child: _numField(
                    '',
                    _transferCtrl,
                    hint: 'Nominal transfer',
                    onDone: (v) =>
                        prov.setNominalTransfer(double.tryParse(v) ?? 0),
                    onChanged: (v) =>
                        prov.setNominalTransfer(double.tryParse(v) ?? 0),
                  ),
                ),
              ],
            ),
            if (prov.nominalTransfer > 0) ...[
              const SizedBox(height: 10),
              const Divider(),
              const SizedBox(height: 6),
              const Text(
                'Info Transfer',
                style: TextStyle(fontSize: 12, color: Color(0xFF64748B)),
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: _textField(
                      'Bank',
                      _bankCtrl,
                      onDone: (v) => prov.setInfoTransfer(bankVal: v),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: _textField(
                      'No. Rekening',
                      _noRekCtrl,
                      onDone: (v) => prov.setInfoTransfer(noRekVal: v),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: _textField(
                      'Nama Rekening',
                      _namaRekCtrl,
                      onDone: (v) => prov.setInfoTransfer(namaRekVal: v),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: _textField(
                      'No. Referensi',
                      _noRefCtrl,
                      onDone: (v) => prov.setInfoTransfer(noRefVal: v),
                    ),
                  ),
                ],
              ),
            ],
          ]),
          const SizedBox(height: 16),

          // ── Status bayar ────────────────────────────────────────
          _statusBayar(prov),

          // ── Jatuh tempo — hanya jika belum lunas ───────────────
          if (!prov.isLunas) ...[
            const SizedBox(height: 10),
            _sectionCard('Jatuh Tempo Hutang', Icons.event_outlined, [
              GestureDetector(
                onTap: () async {
                  final now = DateTime.now();
                  final def =
                      prov.jatuhTempo ?? now.add(const Duration(days: 30));
                  final picked = await showDatePicker(
                    context: context,
                    initialDate: def,
                    firstDate: now,
                    lastDate: DateTime(now.year + 2),
                    locale: const Locale('id', 'ID'),
                    builder: (c, child) => Theme(
                      data: Theme.of(c).copyWith(
                        colorScheme: const ColorScheme.light(
                          primary: _green,
                          onPrimary: Colors.white,
                        ),
                      ),
                      child: child!,
                    ),
                  );
                  if (picked != null) prov.setJatuhTempo(picked);
                },
                child: Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 12,
                  ),
                  decoration: BoxDecoration(
                    color: Colors.grey.shade50,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: Colors.grey.shade300),
                  ),
                  child: Row(
                    children: [
                      Icon(
                        Icons.calendar_today_outlined,
                        size: 16,
                        color: _green,
                      ),
                      const SizedBox(width: 10),
                      Text(
                        prov.jatuhTempo != null
                            ? DateFormat(
                                'dd MMMM yyyy',
                                'id_ID',
                              ).format(prov.jatuhTempo!)
                            : DateFormat('dd MMMM yyyy', 'id_ID').format(
                                DateTime.now().add(const Duration(days: 30)),
                              ),
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const Spacer(),
                      Icon(
                        Icons.edit_calendar_outlined,
                        size: 16,
                        color: _green,
                      ),
                    ],
                  ),
                ),
              ),
            ]),
          ],
          const SizedBox(height: 20),

          // ── Tombol Preview Nota ─────────────────────────────────
          SizedBox(
            width: double.infinity,
            height: 52,
            child: ElevatedButton.icon(
              onPressed: widget.onNext,
              icon: const Icon(Icons.receipt_long_outlined),
              label: const Text(
                'Preview Nota →',
                style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
              ),
            ),
          ),
        ],
      ),
    );
  }

  // ── Ringkasan subtotal + diskon item ─────────────────────────
  Widget _ringkasanItem(PenjualanProvider prov) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        children: [
          _infoRow(
            'Subtotal (${prov.cartItems.length} item)',
            'Rp ${_fmt.format(prov.subtotal)}',
            const Color(0xFF1E293B),
          ),
          if (prov.diskonItemTotal > 0)
            _infoRow(
              'Diskon Item',
              '- Rp ${_fmt.format(prov.diskonItemTotal)}',
              Colors.red,
            ),
        ],
      ),
    );
  }

  // ── Status bayar: total bayar, kembalian/sisa, badge lunas ───
  Widget _statusBayar(PenjualanProvider prov) {
    final isLunas = prov.isLunas;
    final color = isLunas ? _green : Colors.orange;
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.06),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: color.withValues(alpha: 0.3)),
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Total Bayar',
                style: TextStyle(fontSize: 13, color: Color(0xFF64748B)),
              ),
              Text(
                'Rp ${_fmt.format(prov.totalBayar)}',
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: 6),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                isLunas ? 'Kembalian' : 'Sisa Tagihan',
                style: TextStyle(fontSize: 13, color: color),
              ),
              Text(
                'Rp ${_fmt.format(isLunas ? prov.kembali : prov.sisaTagihan)}',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.bold,
                  color: color,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 6),
            decoration: BoxDecoration(
              color: color,
              borderRadius: BorderRadius.circular(8),
            ),
            child: Text(
              isLunas ? '✓  LUNAS' : '⚠  BELUM LUNAS',
              textAlign: TextAlign.center,
              style: const TextStyle(
                color: Colors.white,
                fontWeight: FontWeight.bold,
                fontSize: 13,
              ),
            ),
          ),
        ],
      ),
    );
  }

  // ── Card section dengan header ikon + judul ──────────────────
  Widget _sectionCard(String title, IconData icon, List<Widget> children) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, size: 16, color: _green),
              const SizedBox(width: 6),
              Text(
                title,
                style: const TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                  color: Color(0xFF1E293B),
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          ...children,
        ],
      ),
    );
  }

  // ── Baris info label–value ───────────────────────────────────
  Widget _infoRow(String label, String value, Color color) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
          ),
          Text(
            value,
            style: TextStyle(
              fontSize: 13,
              fontWeight: FontWeight.w600,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  // ── Input angka dengan label opsional ────────────────────────
  Widget _numField(
    String label,
    TextEditingController ctrl, {
    required void Function(String) onDone,
    void Function(String)? onChanged,
    String hint = '0',
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (label.isNotEmpty) ...[
          Text(
            label,
            style: const TextStyle(fontSize: 11, color: Color(0xFF64748B)),
          ),
          const SizedBox(height: 4),
        ],
        TextField(
          controller: ctrl,
          keyboardType: TextInputType.number,
          inputFormatters: [FilteringTextInputFormatter.digitsOnly],
          onChanged: onChanged,
          onEditingComplete: () {
            onDone(ctrl.text);
            FocusScope.of(context).unfocus();
          },
          onTapOutside: (_) {
            onDone(ctrl.text);
            FocusScope.of(context).unfocus();
          },
          style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600),
          decoration: InputDecoration(
            isDense: true,
            hintText: hint,
            hintStyle: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.normal,
              color: Colors.grey.shade400,
            ),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 12,
              vertical: 10,
            ),
            filled: true,
            fillColor: Colors.grey.shade50,
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: const OutlineInputBorder(
              borderRadius: BorderRadius.all(Radius.circular(10)),
              borderSide: BorderSide(color: _green, width: 1.5),
            ),
          ),
        ),
      ],
    );
  }

  // ── Input teks (info bank) ───────────────────────────────────
  Widget _textField(
    String label,
    TextEditingController ctrl, {
    required void Function(String) onDone,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: const TextStyle(fontSize: 11, color: Color(0xFF64748B)),
        ),
        const SizedBox(height: 4),
        TextField(
          controller: ctrl,
          onEditingComplete: () {
            onDone(ctrl.text);
            FocusScope.of(context).unfocus();
          },
          onTapOutside: (_) {
            onDone(ctrl.text);
            FocusScope.of(context).unfocus();
          },
          style: const TextStyle(fontSize: 13),
          decoration: InputDecoration(
            isDense: true,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 12,
              vertical: 10,
            ),
            filled: true,
            fillColor: Colors.grey.shade50,
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: const OutlineInputBorder(
              borderRadius: BorderRadius.all(Radius.circular(10)),
              borderSide: BorderSide(color: _green, width: 1.5),
            ),
          ),
        ),
      ],
    );
  }

  // ── 1 baris: ikon | label | field % | field Rp | hasil ───────
  Widget _inlineNumRow({
    required IconData icon,
    required String label,
    required TextEditingController ctrlPersen,
    required TextEditingController ctrlRp,
    required void Function(String) onChangedPersen,
    required void Function(String) onChangedRp,
    String hintPersen = '%',
    String hintRp = 'Rp',
    String? resultLabel,
    Color resultColor = Colors.grey,
  }) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        Icon(icon, size: 15, color: _green),
        const SizedBox(width: 6),
        SizedBox(
          width: 52,
          child: Text(
            label,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: Color(0xFF1E293B),
            ),
          ),
        ),
        const SizedBox(width: 6),
        SizedBox(
          width: 64,
          child: _numFieldCompact(
            hintPersen,
            ctrlPersen,
            onDone: onChangedPersen,
            onChanged: onChangedPersen,
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: _numFieldCompact(
            hintRp,
            ctrlRp,
            onDone: onChangedRp,
            onChanged: onChangedRp,
          ),
        ),
        if (resultLabel != null) ...[
          const SizedBox(width: 8),
          SizedBox(
            width: 80,
            child: Text(
              resultLabel,
              textAlign: TextAlign.right,
              style: TextStyle(
                fontSize: 11,
                fontWeight: FontWeight.w600,
                color: resultColor,
              ),
            ),
          ),
        ] else
          const SizedBox(width: 88),
      ],
    );
  }

  // ── TextField angka kompak (tanpa label di atas) ─────────────
  Widget _numFieldCompact(
    String hint,
    TextEditingController ctrl, {
    required void Function(String) onDone,
    void Function(String)? onChanged,
  }) {
    return TextField(
      controller: ctrl,
      keyboardType: TextInputType.number,
      inputFormatters: [FilteringTextInputFormatter.digitsOnly],
      textAlign: TextAlign.center,
      onChanged: onChanged,
      onEditingComplete: () {
        onDone(ctrl.text);
        FocusScope.of(context).unfocus();
      },
      onTapOutside: (_) {
        onDone(ctrl.text);
        FocusScope.of(context).unfocus();
      },
      style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
      decoration: InputDecoration(
        isDense: true,
        hintText: hint,
        hintStyle: TextStyle(fontSize: 12, color: Colors.grey.shade400),
        contentPadding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
        filled: true,
        fillColor: Colors.grey.shade50,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        focusedBorder: const OutlineInputBorder(
          borderRadius: BorderRadius.all(Radius.circular(8)),
          borderSide: BorderSide(color: _green, width: 1.5),
        ),
      ),
    );
  }

  // ── Dropdown akun COA (KAS / BANK) ───────────────────────────
  Widget _akunDropdown(
    String label,
    List<Map<String, dynamic>> list,
    Map<String, dynamic>? selected,
    void Function(Map<String, dynamic>?) onChanged,
  ) {
    return DropdownButtonFormField<Map<String, dynamic>>(
      initialValue:
          selected != null &&
              list.any((a) => a['KODE_AKUN'] == selected['KODE_AKUN'])
          ? selected
          : null,
      isExpanded: true,
      hint: Text(
        label,
        style: TextStyle(fontSize: 13, color: Colors.grey.shade400),
      ),
      decoration: InputDecoration(
        isDense: true,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 12,
          vertical: 10,
        ),
        filled: true,
        fillColor: Colors.grey.shade50,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        focusedBorder: const OutlineInputBorder(
          borderRadius: BorderRadius.all(Radius.circular(10)),
          borderSide: BorderSide(color: _green, width: 1.5),
        ),
      ),
      style: const TextStyle(fontSize: 13, color: Color(0xFF1E293B)),
      items: list
          .map(
            (a) => DropdownMenuItem(
              value: a,
              child: Text(
                '${a['KODE_AKUN']} - ${a['NAMA_AKUN']}',
                overflow: TextOverflow.ellipsis,
              ),
            ),
          )
          .toList(),
      onChanged: onChanged,
    );
  }
}
