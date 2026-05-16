import 'package:flutter/material.dart';
import '../services/role_mapping_service.dart';

class RoleMappingScreen extends StatefulWidget {
  const RoleMappingScreen({super.key});

  @override
  State<RoleMappingScreen> createState() => _RoleMappingScreenState();
}

class _RoleMappingScreenState extends State<RoleMappingScreen> {
  static const _green = Color(0xFF16A34A);

  final Map<String, TextEditingController> _ctrls = {};
  bool _loading = true;
  bool _dirty = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void dispose() {
    for (final c in _ctrls.values) {
      c.dispose();
    }
    super.dispose();
  }

  Future<void> _load() async {
    final m = await RoleMappingService.load();
    setState(() {
      for (final key in m.keys) {
        _ctrls[key] = TextEditingController(text: m[key]);
      }
      _loading = false;
    });
  }

  Future<void> _save() async {
    // Kumpulkan nilai dari controller
    final updated = <String, String>{};
    for (final key in _ctrls.keys) {
      final val = _ctrls[key]!.text.trim();
      updated[key] = val.isNotEmpty
          ? val
          : RoleMappingService.defaultMapping[key] ?? '';
    }
    await RoleMappingService.save(updated);
    if (!mounted) return;
    setState(() => _dirty = false);
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Mapping disimpan'),
        backgroundColor: _green,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  Future<void> _reset() async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Reset ke Default?'),
        content: const Text(
          'Semua nama role akan dikembalikan ke nilai default '
          'sesuai FormGeneralSetting VB.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Batal'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text('Reset'),
          ),
        ],
      ),
    );
    if (ok != true) return;
    await RoleMappingService.reset();
    for (final key in _ctrls.keys) {
      _ctrls[key]!.text = RoleMappingService.defaultMapping[key] ?? '';
    }
    setState(() => _dirty = false);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Mapping direset ke default'),
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(
        title: const Text('Mapping Role Hak Akses'),
        actions: [
          IconButton(
            icon: const Icon(Icons.restore),
            tooltip: 'Reset ke Default',
            onPressed: _reset,
          ),
          if (_dirty)
            TextButton(
              onPressed: _save,
              child: const Text(
                'Simpan',
                style: TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
        ],
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : Column(
              children: [
                // Info banner
                Container(
                  width: double.infinity,
                  margin: const EdgeInsets.fromLTRB(16, 12, 16, 0),
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: Colors.blue.shade50,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: Colors.blue.shade200),
                  ),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Icon(
                        Icons.info_outline,
                        size: 16,
                        color: Colors.blue.shade700,
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          'Isi kolom "Nama Role di DB" dengan teks persis '
                          'seperti yang tertulis di Label FormGeneralSetting '
                          'pada aplikasi desktop VB. Nilai ini dipakai untuk '
                          'query ke tabel hakaksesuser.',
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.blue.shade800,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 8),

                // List mapping
                Expanded(
                  child: ListView.separated(
                    padding: const EdgeInsets.fromLTRB(16, 8, 16, 80),
                    itemCount: _ctrls.length,
                    separatorBuilder: (_, _) => const SizedBox(height: 10),
                    itemBuilder: (_, i) {
                      final key = _ctrls.keys.elementAt(i);
                      final label = RoleMappingService.keyLabels[key] ?? key;
                      final def = RoleMappingService.defaultMapping[key] ?? '';
                      return _MappingCard(
                        label: label,
                        jsonKey: key,
                        ctrl: _ctrls[key]!,
                        defaultValue: def,
                        onChanged: () => setState(() => _dirty = true),
                      );
                    },
                  ),
                ),
              ],
            ),
      floatingActionButton: _dirty
          ? FloatingActionButton.extended(
              onPressed: _save,
              backgroundColor: _green,
              icon: const Icon(Icons.save_outlined),
              label: const Text('Simpan'),
            )
          : null,
    );
  }
}

class _MappingCard extends StatelessWidget {
  final String label;
  final String jsonKey;
  final TextEditingController ctrl;
  final String defaultValue;
  final VoidCallback onChanged;

  const _MappingCard({
    required this.label,
    required this.jsonKey,
    required this.ctrl,
    required this.defaultValue,
    required this.onChanged,
  });

  static const _green = Color(0xFF16A34A);

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Label izin
          Row(
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: _green.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(6),
                ),
                child: Text(
                  label,
                  style: const TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                    color: _green,
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  jsonKey,
                  style: TextStyle(
                    fontSize: 10,
                    color: Colors.grey.shade400,
                    fontFamily: 'monospace',
                  ),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),

          // Input nama role
          TextField(
            controller: ctrl,
            onChanged: (_) => onChanged(),
            style: const TextStyle(fontSize: 13),
            decoration: InputDecoration(
              labelText: 'Nama Role di DB',
              labelStyle: TextStyle(fontSize: 12, color: Colors.grey.shade500),
              hintText: defaultValue,
              hintStyle: TextStyle(fontSize: 12, color: Colors.grey.shade300),
              isDense: true,
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 12,
                vertical: 10,
              ),
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
              suffixIcon: ctrl.text != defaultValue
                  ? IconButton(
                      icon: Icon(
                        Icons.restore,
                        size: 16,
                        color: Colors.grey.shade400,
                      ),
                      tooltip: 'Kembalikan ke default',
                      onPressed: () {
                        ctrl.text = defaultValue;
                        onChanged();
                      },
                    )
                  : null,
            ),
          ),

          // Default hint
          const SizedBox(height: 4),
          Text(
            'Default: $defaultValue',
            style: TextStyle(fontSize: 10, color: Colors.grey.shade400),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ],
      ),
    );
  }
}
