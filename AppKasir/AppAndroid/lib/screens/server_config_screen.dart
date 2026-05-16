import 'dart:io';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../services/storage_service.dart';

class ServerConfigScreen extends StatefulWidget {
  const ServerConfigScreen({super.key});

  @override
  State<ServerConfigScreen> createState() => _ServerConfigScreenState();
}

class _ServerConfigScreenState extends State<ServerConfigScreen> with TickerProviderStateMixin {
  final _formKey = GlobalKey<FormState>();
  final _urlController = TextEditingController();
  
  bool _isLoading = false;
  bool _isTestingConnection = false;
  bool _connectionStatus = false;
  String _connectionMessage = 'Belum diuji';
  bool _isScanning = false;
  bool _showIpList = false;
  final List<String> _foundIps = [];
  
  late AnimationController _fadeController;
  late Animation<double> _fadeAnimation;

  @override
  void initState() {
    super.initState();
    _fadeController = AnimationController(
      duration: const Duration(milliseconds: 600),
      vsync: this,
    );
    _fadeAnimation = CurvedAnimation(
      parent: _fadeController,
      curve: Curves.easeOut,
    );
    _fadeController.forward();
    _loadCurrentConfig();
  }

  Future<void> _loadCurrentConfig() async {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    
    if (authProvider.serverUrl != null) {
      final url = authProvider.serverUrl!.replaceAll('http://', '').replaceAll('https://', '').replaceAll('/api', '');
      if (mounted) {
        setState(() {
          _urlController.text = url;
        });
      }
    } else {
      final storedUrl = await StorageService.getServerUrl();
      if (storedUrl != null) {
        final url = storedUrl.replaceAll('http://', '').replaceAll('https://', '').replaceAll('/api', '');
        if (mounted) {
          setState(() {
            _urlController.text = url;
          });
        }
      } else {
        if (mounted) {
          setState(() {
            _urlController.text = '192.168.1.100';
          });
        }
      }
    }
  }

  String? _detectedSubnet;
  
  Future<void> _scanNetwork() async {
    setState(() {
      _isScanning = true;
      _showIpList = true;
      _foundIps.clear();
      _detectedSubnet = null;
    });

    try {
      debugPrint('[Network Scan] 🚀 Starting network scan...');
      
      await _findRouterIp();
      
      if (_detectedSubnet != null) {
        debugPrint('[Network Scan] 🎯 Scanning only subnet: $_detectedSubnet');
        await _scanSingleSubnet(_detectedSubnet!);
      }
    } catch (e) {
      debugPrint('[Network Scan] ❌ Error scanning network: $e');
    } finally {
      if (mounted) {
        setState(() {
          _isScanning = false;
        });
      }
    }
  }

  Future<void> _findRouterIp() async {
    debugPrint('[Network Scan] 🔍 Looking for router/gateway...');
    final List<String> commonGateways = [
      '192.168.1.1', '192.168.0.1', '192.168.2.1', 
      '192.168.100.1', '10.0.0.1', '10.1.1.1', '172.16.0.1'
    ];

    for (final gateway in commonGateways) {
      try {
        final Socket socket = await Socket.connect(
          gateway,
          80,
          timeout: const Duration(milliseconds: 300),
        );
        socket.destroy();
        
        debugPrint('[Network Scan] ✅ Found gateway: $gateway');
        final List<String> parts = gateway.split('.');
        if (parts.length == 4) {
          _detectedSubnet = '${parts[0]}.${parts[1]}.${parts[2]}';
          break;
        }
      } catch (_) {
        try {
          final Socket socket = await Socket.connect(
            gateway,
            443,
            timeout: const Duration(milliseconds: 300),
          );
          socket.destroy();
          
          debugPrint('[Network Scan] ✅ Found gateway (443): $gateway');
          final List<String> parts = gateway.split('.');
          if (parts.length == 4) {
            _detectedSubnet = '${parts[0]}.${parts[1]}.${parts[2]}';
            break;
          }
        } catch (_) {}
      }
    }
  }

  Future<void> _scanSingleSubnet(String subnet) async {
    final List<Future<void>> futures = [];
    
    for (int i = 2; i <= 254; i++) {
      final String targetIp = '$subnet.$i';
      futures.add(_checkIpWithThrottle(targetIp, i));
    }
    
    await Future.wait(futures);
  }

  Future<void> _checkIpWithThrottle(String ip, int index) async {
    await Future.delayed(Duration(milliseconds: index * 5));
    await _checkIp(ip);
  }

  Future<void> _checkIp(String ip) async {
    const List<int> ports = [80, 443, 8080, 3000, 5000];
    
    for (final port in ports) {
      try {
        final Socket socket = await Socket.connect(
          ip,
          port,
          timeout: const Duration(milliseconds: 150),
        );
        socket.destroy();
        
        debugPrint('[Network Scan] ✅ Found IP: $ip:$port');
        if (mounted) {
          setState(() {
            if (!_foundIps.contains(ip)) {
              _foundIps.add(ip);
            }
          });
        }
        return;
      } catch (_) {
      }
    }
  }

  void _selectIp(String ip) {
    setState(() {
      _urlController.text = ip;
      _showIpList = false;
    });
  }

  @override
  void dispose() {
    _urlController.dispose();
    _fadeController.dispose();
    super.dispose();
  }

  Future<void> _testConnection() async {
    debugPrint('[ServerConfig] 🔍 Testing connection...');
    
    if (_urlController.text.trim().isEmpty) {
      _showErrorSnackBar('Masukkan IP Server terlebih dahulu');
      return;
    }

    setState(() {
      _isTestingConnection = true;
      _connectionMessage = 'Mengecek koneksi...';
    });

    try {
      String serverUrl = _urlController.text.trim();
      debugPrint('[ServerConfig] 📡 Input IP: $serverUrl');
      
      if (!serverUrl.startsWith('http')) {
        serverUrl = 'http://$serverUrl/api';
      }
      debugPrint('[ServerConfig] 🔗 Full URL: $serverUrl');
      
      final result = await ApiService.testConnection(serverUrl);
      debugPrint('[ServerConfig] 📋 Test Result: $result');
      
      if (result['status'] == 'success') {
        debugPrint('[ServerConfig] ✅ Connection SUCCESS!');
        setState(() {
          _connectionStatus = true;
          _connectionMessage = 'Koneksi berhasil!';
        });
        _showSuccessSnackBar('Koneksi berhasil!');
      } else {
        debugPrint('[ServerConfig] ❌ Connection FAILED: ${result['message']}');
        setState(() {
          _connectionStatus = false;
          _connectionMessage = result['message'] ?? 'Koneksi gagal';
        });
      }
    } catch (e) {
      debugPrint('[ServerConfig] ❌ Error: $e');
      setState(() {
        _connectionStatus = false;
        _connectionMessage = 'Koneksi gagal: ${e.toString()}';
      });
      _showErrorSnackBar('Koneksi gagal: ${e.toString()}');
    } finally {
      setState(() {
        _isTestingConnection = false;
      });
    }
  }

  Future<void> _saveConfiguration() async {
    debugPrint('[ServerConfig] 💾 Saving configuration...');
    
    if (!_formKey.currentState!.validate()) {
      debugPrint('[ServerConfig] ❌ Form invalid');
      return;
    }

    if (!_connectionStatus) {
      debugPrint('[ServerConfig] ❌ Connection not tested');
      _showErrorSnackBar('Test koneksi terlebih dahulu');
      return;
    }

    setState(() {
      _isLoading = true;
    });

    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    
    try {
      String serverUrl = _urlController.text.trim();
      if (!serverUrl.startsWith('http')) {
        serverUrl = 'http://$serverUrl/api';
      }
      debugPrint('[ServerConfig] 📡 Saving URL: $serverUrl');
      
      final success = await authProvider.configureServer(
        serverUrl,
        '',
        '',
        '',
      );
      
      debugPrint('[ServerConfig] 📊 Save result: $success');
      
      if (success) {
        debugPrint('[ServerConfig] ✅ Configuration saved SUCCESSFULLY!');
        if (mounted) {
          _showSuccessDialog('Konfigurasi server berhasil disimpan!');
        }
      } else {
        debugPrint('[ServerConfig] ❌ Save FAILED: ${authProvider.errorMessage}');
        if (mounted) {
          _showErrorSnackBar(authProvider.errorMessage ?? 'Gagal menyimpan konfigurasi');
        }
      }
    } catch (e) {
      debugPrint('[ServerConfig] ❌ Error saving: $e');
      if (mounted) {
        _showErrorSnackBar('Error: ${e.toString()}');
      }
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Konfigurasi Server'),
        backgroundColor: const Color(0xFF2563EB),
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              const Color(0xFFF8FAFC),
              const Color(0xFFF1F5F9),
            ],
          ),
        ),
        child: SafeArea(
          child: FadeTransition(
            opacity: _fadeAnimation,
            child: SingleChildScrollView(
              physics: const BouncingScrollPhysics(),
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      _buildHeader(),
                      const SizedBox(height: 20),
                      _buildServerUrlSection(),
                      if (_showIpList) ...[
                        const SizedBox(height: 12),
                        _buildIpList(),
                      ],
                      const SizedBox(height: 16),
                      _buildConnectionStatus(),
                      const SizedBox(height: 24),
                      _buildSaveButton(),
                      const SizedBox(height: 16),
                      _buildInfoCard(),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Column(
      children: [
        Container(
          padding: const EdgeInsets.all(18),
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [Color(0xFF2563EB), Color(0xFF1E40AF)],
            ),
            shape: BoxShape.circle,
            boxShadow: [
              BoxShadow(
                color: const Color(0xFF2563EB).withValues(alpha:0.3),
                blurRadius: 18,
                offset: const Offset(0, 6),
              ),
            ],
          ),
          child: const Icon(
            Icons.settings_ethernet,
            size: 44,
            color: Colors.white,
          ),
        ),
        const SizedBox(height: 16),
        const Text(
          'Konfigurasi Server',
          style: TextStyle(
            fontSize: 24,
            fontWeight: FontWeight.bold,
            color: Color(0xFF1E293B),
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 4),
        Text(
          'Atur koneksi ke server API',
          style: TextStyle(
            fontSize: 14,
            color: Colors.grey.shade600,
          ),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }

  Widget _buildServerUrlSection() {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha:0.05),
            blurRadius: 12,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Alamat IP Server',
              style: TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.w600,
                color: Colors.grey.shade800,
              ),
            ),
            const SizedBox(height: 10),
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _urlController,
                    decoration: InputDecoration(
                      hintText: '192.168.1.100',
                      prefixIcon: Icon(Icons.dns_outlined, color: Colors.grey.shade600),
                      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: BorderSide(color: Colors.grey.shade300),
                      ),
                      enabledBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: BorderSide(color: Colors.grey.shade300),
                      ),
                      focusedBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: const BorderSide(color: Color(0xFF2563EB), width: 2),
                      ),
                    ),
                    validator: (value) {
                      if (value == null || value.isEmpty) {
                        return 'Masukkan IP Server';
                      }
                      return null;
                    },
                    enabled: !_isTestingConnection && !_isLoading,
                  ),
                ),
                const SizedBox(width: 10),
                GestureDetector(
                  onTap: _isScanning || _isTestingConnection ? null : _scanNetwork,
                  child: Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: _isScanning 
                          ? Colors.grey.shade200 
                          : const Color(0xFF2563EB).withValues(alpha:0.1),
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(
                        color: _isScanning 
                            ? Colors.grey.shade300 
                            : const Color(0xFF2563EB).withValues(alpha:0.3),
                      ),
                    ),
                    child: _isScanning
                        ? SizedBox(
                            width: 24,
                            height: 24,
                            child: CircularProgressIndicator(
                              strokeWidth: 2.5,
                              valueColor: AlwaysStoppedAnimation<Color>(Colors.grey.shade600),
                            ),
                          )
                        : Icon(
                            Icons.wifi_find_outlined,
                            size: 24,
                            color: const Color(0xFF2563EB),
                          ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: _isTestingConnection || _isLoading ? null : _testConnection,
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFFF59E0B),
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  elevation: 1,
                ),
                icon: _isTestingConnection
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(
                          strokeWidth: 2.5,
                          valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                        ),
                      )
                    : const Icon(Icons.wifi_tethering, size: 20),
                label: _isTestingConnection
                    ? const Text('MENGECEK...')
                    : const Text('UJI KONEKSI'),
              ),
            ),
          ],
        ),
    );
  }

  Widget _buildIpList() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            child: Row(
              children: [
                Icon(
                  _isScanning ? Icons.sync_outlined : Icons.devices_outlined,
                  color: _isScanning ? Colors.amber.shade600 : Colors.grey.shade600,
                  size: 20,
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    _isScanning 
                        ? 'Mencari perangkat di jaringan... (${_foundIps.length})' 
                        : 'Perangkat ditemukan (${_foundIps.length})',
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                      color: _isScanning ? Colors.amber.shade700 : Colors.grey.shade700,
                    ),
                  ),
                ),
              ],
            ),
          ),
          const Divider(height: 1),
          if (_isScanning && _foundIps.isEmpty) ...[
            const Padding(
              padding: EdgeInsets.symmetric(vertical: 30),
              child: Column(
                children: [
                  SizedBox(
                    width: 30,
                    height: 30,
                    child: CircularProgressIndicator(strokeWidth: 2.5),
                  ),
                  SizedBox(height: 12),
                  Text(
                    'Scanning jaringan...',
                    style: TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            ),
          ] else if (_foundIps.isEmpty) ...[
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 30),
              child: Column(
                children: [
                  const Icon(Icons.phonelink_off_outlined, size: 40, color: Colors.grey),
                  const SizedBox(height: 12),
                  const Text(
                    'Tidak ada perangkat ditemukan',
                    style: TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            ),
          ] else ...[
            ..._foundIps.map((ip) => ListTile(
                  dense: true,
                  leading: Icon(Icons.computer_outlined, color: Colors.grey.shade500, size: 20),
                  title: Text(
                    ip,
                    style: TextStyle(fontSize: 14, color: Colors.grey.shade800),
                  ),
                  onTap: () => _selectIp(ip),
                  visualDensity: VisualDensity.compact,
                  contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 0),
                )),
          ],
        ],
      ),
    );
  }

  Widget _buildConnectionStatus() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        color: _connectionStatus 
            ? const Color(0xFF10B981).withValues(alpha:0.1) 
            : Colors.grey.shade100,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: _connectionStatus 
              ? const Color(0xFF10B981).withValues(alpha:0.3) 
              : Colors.grey.shade300,
        ),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: _connectionStatus 
                  ? const Color(0xFF10B981).withValues(alpha:0.15)
                  : Colors.grey.shade200,
              shape: BoxShape.circle,
            ),
            child: Icon(
              _connectionStatus ? Icons.check_circle : Icons.info_outline,
              color: _connectionStatus ? const Color(0xFF10B981) : Colors.grey.shade600,
              size: 20,
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  _connectionStatus ? 'Koneksi Berhasil' : 'Status',
                  style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: _connectionStatus ? const Color(0xFF10B981) : Colors.grey.shade700,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  _connectionMessage,
                  style: TextStyle(
                    fontSize: 12,
                    color: _connectionStatus ? const Color(0xFF10B981).withValues(alpha:0.8) : Colors.grey.shade600,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSaveButton() {
    return ElevatedButton.icon(
      onPressed: (_isLoading || !_connectionStatus) ? null : _saveConfiguration,
      style: ElevatedButton.styleFrom(
        backgroundColor: const Color(0xFF2563EB),
        foregroundColor: Colors.white,
        padding: const EdgeInsets.symmetric(vertical: 16),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        elevation: 2,
        disabledBackgroundColor: Colors.grey.shade300,
      ),
      icon: _isLoading
          ? const SizedBox(
              height: 20,
              width: 20,
              child: CircularProgressIndicator(
                strokeWidth: 2.5,
                valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
              ),
            )
          : const Icon(Icons.save_outlined, size: 20),
      label: const Text(
        'SIMPAN KONFIGURASI',
        style: TextStyle(fontSize: 15, fontWeight: FontWeight.w600),
      ),
    );
  }

  Widget _buildInfoCard() {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFF2563EB).withValues(alpha:0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: const Color(0xFF2563EB).withValues(alpha:0.2)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(Icons.info_outline, color: const Color(0xFF2563EB), size: 20),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              'Test koneksi sebelum menyimpan. Pastikan perangkat berada di jaringan yang sama dengan server.',
              style: TextStyle(
                fontSize: 12,
                color: const Color(0xFF2563EB).withValues(alpha:0.85),
                height: 1.3,
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showSuccessSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(Icons.check_circle, color: Colors.white),
            const SizedBox(width: 10),
            Expanded(child: Text(message)),
          ],
        ),
        backgroundColor: const Color(0xFF10B981),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
      ),
    );
  }

  void _showErrorSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(Icons.error_outline, color: Colors.white),
            const SizedBox(width: 10),
            Expanded(child: Text(message)),
          ],
        ),
        backgroundColor: Colors.red.shade600,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
      ),
    );
  }

  void _showSuccessDialog(String message) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(20),
        ),
        title: Row(
          children: [
            const Icon(Icons.check_circle, color: Color(0xFF10B981)),
            const SizedBox(width: 12),
            const Text('Berhasil'),
          ],
        ),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              Navigator.of(context).pop();
            },
            style: TextButton.styleFrom(
              foregroundColor: const Color(0xFF2563EB),
            ),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }
}
