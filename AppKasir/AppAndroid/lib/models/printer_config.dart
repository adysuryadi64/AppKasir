import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class PrinterConfig {
  final String deviceAddress;
  final String deviceName;
  final int paperWidth; // 58 atau 80 (mm)
  final int charsPerLine; // 32 atau 48
  final int model; // 1-8
  final bool autocut;
  final int copies;

  const PrinterConfig({
    required this.deviceAddress,
    required this.deviceName,
    this.paperWidth = 58,
    this.charsPerLine = 32,
    this.model = 1,
    this.autocut = true,
    this.copies = 1,
  });

  static const _key = 'printer_config';

  factory PrinterConfig.fromJson(Map<String, dynamic> j) => PrinterConfig(
    deviceAddress: j['deviceAddress'] ?? '',
    deviceName: j['deviceName'] ?? '',
    paperWidth: j['paperWidth'] ?? 58,
    charsPerLine: j['charsPerLine'] ?? 32,
    model: j['model'] ?? 1,
    autocut: j['autocut'] ?? true,
    copies: j['copies'] ?? 1,
  );

  Map<String, dynamic> toJson() => {
    'deviceAddress': deviceAddress,
    'deviceName': deviceName,
    'paperWidth': paperWidth,
    'charsPerLine': charsPerLine,
    'model': model,
    'autocut': autocut,
    'copies': copies,
  };

  bool get isConfigured => deviceAddress.isNotEmpty;

  static Future<PrinterConfig?> load() async {
    final prefs = await SharedPreferences.getInstance();
    final s = prefs.getString(_key);
    if (s == null || s.isEmpty) return null;
    try {
      return PrinterConfig.fromJson(jsonDecode(s) as Map<String, dynamic>);
    } catch (_) {
      return null;
    }
  }

  Future<void> save() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_key, jsonEncode(toJson()));
  }

  static Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_key);
  }

  PrinterConfig copyWith({
    String? deviceAddress,
    String? deviceName,
    int? paperWidth,
    int? charsPerLine,
    int? model,
    bool? autocut,
    int? copies,
  }) => PrinterConfig(
    deviceAddress: deviceAddress ?? this.deviceAddress,
    deviceName: deviceName ?? this.deviceName,
    paperWidth: paperWidth ?? this.paperWidth,
    charsPerLine: charsPerLine ?? this.charsPerLine,
    model: model ?? this.model,
    autocut: autocut ?? this.autocut,
    copies: copies ?? this.copies,
  );
}
