import 'package:flutter/material.dart';
import 'package:flutter_thermal_printer/utils/printer.dart';
import '../models/printer_config.dart';
import '../services/thermal_print_service.dart';

class PrinterSettingsScreen extends StatefulWidget {
  const PrinterSettingsScreen({super.key});

  @override
  State<PrinterSettingsScreen> createState() => _PrinterSettingsScreenState();
}

class _PrinterSettingsScreenState extends State<PrinterSettingsScreen> {
  static const _green = Color(0xFF16A34A);

  PrinterConfig? _config;
  List<Printer> _devices = [];
  bool _isScanning = false;
  bool _isSaving = false;

  // Form state
  Printer? _selectedDevice;
  int _paperWidth = 58;
  int _model = 1;
  bool _autocut = true;

  @override
  void initState() {
    super.initState();
    _loadConfig();
    _startScan();
  }

  Future<void> _loadConfig() async {
    final cfg = await PrinterConfig.load();
    if (mounted) setState(() => _config = cfg);
  }

  Future<void> _startScan() async {
    setState(() {
      _isScanning = true;
      _devices = [];
    });

    ThermalPrintService.scanStream.listen((devices) {
      if (mounted) {
        setState(() {
          _devices = devices;
          _isScanning = false;
        });
      }
    });
  }

  Future<void> _save() async {
    if (_selectedDevice == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Pilih printer terlebih dahulu'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }
    setState(() => _isSaving = true);
    final cfg = PrinterConfig(
      deviceAddress: _selectedDevice!.address ?? '',
      deviceName: _selectedDevice!.name ?? '',
      paperWidth: _paperWidth,
      charsPerLine: _paperWidth == 80 ? 48 : 32,
      model: _model,
      autocut: _autocut,
    );
    await cfg.save();
    if (mounted) {
      setState(() {
        _config = cfg;
        _isSaving = false;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Pengaturan printer disimpan'),
          backgroundColor: _green,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(title: const Text('Pengaturan Printer')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Status printer tersimpan
            if (_config != null && _config!.isConfigured)
              Container(
                padding: const EdgeInsets.all(12),
                margin: const EdgeInsets.only(bottom: 16),
                decoration: BoxDecoration(
                  color: _green.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: _green.withValues(alpha: 0.3)),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.print, color: _green, size: 20),
                    const SizedBox(width: 10),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            _config!.deviceName,
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 13,
                            ),
                          ),
                          Text(
                            '${_config!.paperWidth}mm · Model ${_config!.model}',
                            style: TextStyle(
                              fontSize: 11,
                              color: Colors.grey.shade500,
                            ),
                          ),
                        ],
                      ),
                    ),
                    TextButton(
                      onPressed: () async {
                        await PrinterConfig.clear();
                        _loadConfig();
                      },
                      child: const Text(
                        'Hapus',
                        style: TextStyle(color: Colors.red),
                      ),
                    ),
                  ],
                ),
              ),

            // Scan
            _sectionLabel('Pilih Printer Bluetooth'),
            const SizedBox(height: 8),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                onPressed: _isScanning ? null : _startScan,
                icon: _isScanning
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.bluetooth_searching),
                label: Text(_isScanning ? 'Mencari...' : 'Scan Printer'),
              ),
            ),
            const SizedBox(height: 10),

            // Daftar device
            if (_devices.isNotEmpty)
              Container(
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: ListView.separated(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemCount: _devices.length,
                  separatorBuilder: (context, index) =>
                      const Divider(height: 1),
                  itemBuilder: (_, i) {
                    final d = _devices[i];
                    final selected = _selectedDevice?.address == d.address;
                    return ListTile(
                      dense: true,
                      leading: Icon(
                        Icons.print_outlined,
                        color: selected ? _green : Colors.grey,
                      ),
                      title: Text(
                        d.name ?? 'Unknown',
                        style: TextStyle(
                          fontWeight: selected
                              ? FontWeight.bold
                              : FontWeight.normal,
                        ),
                      ),
                      subtitle: Text(
                        d.address ?? '',
                        style: const TextStyle(fontSize: 11),
                      ),
                      trailing: selected
                          ? const Icon(Icons.check_circle, color: _green)
                          : null,
                      onTap: () => setState(() => _selectedDevice = d),
                    );
                  },
                ),
              )
            else if (!_isScanning)
              Center(
                child: Text(
                  'Tekan Scan untuk mencari printer',
                  style: TextStyle(color: Colors.grey.shade400, fontSize: 13),
                ),
              ),

            const SizedBox(height: 20),

            // Lebar kertas
            _sectionLabel('Lebar Kertas'),
            const SizedBox(height: 8),
            Row(
              children: [58, 80].map((w) {
                final active = _paperWidth == w;
                return Expanded(
                  child: GestureDetector(
                    onTap: () => setState(() => _paperWidth = w),
                    child: Container(
                      margin: const EdgeInsets.only(right: 8),
                      padding: const EdgeInsets.symmetric(vertical: 12),
                      decoration: BoxDecoration(
                        color: active ? _green : Colors.white,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(
                          color: active ? _green : Colors.grey.shade300,
                        ),
                      ),
                      child: Text(
                        '${w}mm',
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontWeight: FontWeight.bold,
                          color: active ? Colors.white : Colors.black87,
                        ),
                      ),
                    ),
                  ),
                );
              }).toList(),
            ),

            const SizedBox(height: 20),

            // Model nota
            _sectionLabel('Model Nota'),
            const SizedBox(height: 8),
            _modelNotaTable(),

            const SizedBox(height: 20),

            // Autocut
            SwitchListTile(
              value: _autocut,
              onChanged: (v) => setState(() => _autocut = v),
              title: const Text('Auto Cut', style: TextStyle(fontSize: 14)),
              subtitle: const Text(
                'Potong kertas otomatis setelah cetak',
                style: TextStyle(fontSize: 12),
              ),
              activeThumbColor: _green,
              contentPadding: EdgeInsets.zero,
            ),

            const SizedBox(height: 24),

            SizedBox(
              width: double.infinity,
              height: 52,
              child: ElevatedButton.icon(
                onPressed: _isSaving ? null : _save,
                icon: _isSaving
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: Colors.white,
                        ),
                      )
                    : const Icon(Icons.save_outlined),
                label: const Text(
                  'Simpan Pengaturan',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _sectionLabel(String text) => Text(
    text,
    style: const TextStyle(
      fontSize: 12,
      fontWeight: FontWeight.w600,
      color: Color(0xFF64748B),
      letterSpacing: 0.5,
    ),
  );

  Widget _modelNotaTable() {
    const models = [
      [1, true, true, true],
      [2, true, true, false],
      [3, true, false, true],
      [4, true, false, false],
      [5, false, true, true],
      [6, false, true, false],
      [7, false, false, true],
      [8, false, false, false],
    ];
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        children: [
          // Header
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            child: Row(
              children: [
                const SizedBox(width: 50),
                Expanded(
                  child: Text(
                    'Header Kolom',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                    textAlign: TextAlign.center,
                  ),
                ),
                Expanded(
                  child: Text(
                    'Diskon',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                    textAlign: TextAlign.center,
                  ),
                ),
                Expanded(
                  child: Text(
                    'Sisa Hutang',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                    textAlign: TextAlign.center,
                  ),
                ),
              ],
            ),
          ),
          const Divider(height: 1),
          ...models.map((m) {
            final no = m[0] as int;
            final active = _model == no;
            return InkWell(
              onTap: () => setState(() => _model = no),
              child: Container(
                color: active
                    ? const Color(0xFF16A34A).withValues(alpha: 0.06)
                    : null,
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 10,
                ),
                child: Row(
                  children: [
                    SizedBox(
                      width: 50,
                      child: Text(
                        'Model $no',
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: active
                              ? FontWeight.bold
                              : FontWeight.normal,
                          color: active
                              ? const Color(0xFF16A34A)
                              : Colors.black87,
                        ),
                      ),
                    ),
                    Expanded(
                      child: Icon(
                        m[1] as bool ? Icons.check : Icons.close,
                        size: 16,
                        color: m[1] as bool
                            ? const Color(0xFF16A34A)
                            : Colors.grey.shade300,
                        textDirection: TextDirection.ltr,
                      ).apply(textDirection: TextDirection.ltr),
                    ),
                    Expanded(
                      child: Icon(
                        m[2] as bool ? Icons.check : Icons.close,
                        size: 16,
                        color: m[2] as bool
                            ? const Color(0xFF16A34A)
                            : Colors.grey.shade300,
                      ),
                    ),
                    Expanded(
                      child: Icon(
                        m[3] as bool ? Icons.check : Icons.close,
                        size: 16,
                        color: m[3] as bool
                            ? const Color(0xFF16A34A)
                            : Colors.grey.shade300,
                      ),
                    ),
                  ],
                ),
              ),
            );
          }),
        ],
      ),
    );
  }
}

extension on Icon {
  Icon apply({TextDirection? textDirection}) => this;
}
