import 'dart:async';
import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import 'storage_service.dart';

class ApiService {
  static const Duration timeout = Duration(seconds: 30);

  // ── Core HTTP ─────────────────────────────────────────────────
  static Future<http.Response> _makeRequest(
    String endpoint, {
    String method = 'GET',
    Map<String, dynamic>? body,
    Duration? customTimeout,
    Map<String, String>? extraHeaders,
    String? overrideBaseUrl,
  }) async {
    final baseUrl = overrideBaseUrl ?? await StorageService.getServerUrl();
    if (baseUrl == null || baseUrl.isEmpty) {
      debugPrint('[API] ❌ _makeRequest($endpoint) — baseUrl null/kosong');
      throw Exception('Server belum dikonfigurasi');
    }

    final cleanBase = baseUrl.endsWith('/')
        ? baseUrl.substring(0, baseUrl.length - 1)
        : baseUrl;
    final cleanEp = endpoint.startsWith('/') ? endpoint.substring(1) : endpoint;
    final url = Uri.parse('$cleanBase/$cleanEp');

    final token = StorageService.getToken();
    final hasToken = token != null && token.isNotEmpty;
    final headers = <String, String>{
      'Content-Type': 'application/json',
      if (hasToken) 'Authorization': 'Bearer $token',
      ...?extraHeaders,
    };

    debugPrint('[API] ➡️  $method $url');
    debugPrint(
      '[API]     token: ${hasToken ? '✅ ada (${token.substring(0, 8)}...)' : '⚠️  tidak ada'}',
    );
    if (body != null) {
      debugPrint(
        '[API]     body: ${jsonEncode(body).substring(0, (jsonEncode(body).length > 120 ? 120 : jsonEncode(body).length))}...',
      );
    }

    final t = customTimeout ?? timeout;

    try {
      http.Response response;
      final sw = Stopwatch()..start();
      switch (method) {
        case 'POST':
          response = await http
              .post(url, headers: headers, body: jsonEncode(body))
              .timeout(t);
          break;
        case 'PUT':
          response = await http
              .put(
                url,
                headers: headers,
                body: body != null ? jsonEncode(body) : null,
              )
              .timeout(t);
          break;
        case 'DELETE':
          response = await http.delete(url, headers: headers).timeout(t);
          break;
        default:
          response = await http.get(url, headers: headers).timeout(t);
      }
      sw.stop();
      debugPrint(
        '[API] ✅  ${response.statusCode} (${sw.elapsedMilliseconds}ms) ← $cleanEp',
      );
      if (response.statusCode != 200) {
        debugPrint(
          '[API]     body: ${response.body.substring(0, (response.body.length > 200 ? 200 : response.body.length))}',
        );
      }
      return response;
    } on TimeoutException {
      debugPrint('[API] ⏱️  TIMEOUT $cleanEp (>${t.inSeconds}s)');
      throw Exception('Timeout — server tidak merespons dalam ${t.inSeconds}s');
    } catch (e) {
      debugPrint('[API] ❌  ERROR $cleanEp → $e');
      throw Exception('Koneksi gagal: $e');
    }
  }

  // ── Helper: parse response ────────────────────────────────────
  static Map<String, dynamic> _parse(http.Response r) {
    if (r.statusCode == 200 || r.statusCode == 201) {
      try {
        final decoded = jsonDecode(r.body) as Map<String, dynamic>;
        if (decoded['status'] != 'success') {
          debugPrint(
            '[API] ⚠️  status=${decoded['status']} message=${decoded['message']}',
          );
        }
        return decoded;
      } catch (e) {
        debugPrint(
          '[API] ❌ _parse: bukan JSON valid → ${r.body.substring(0, (r.body.length > 100 ? 100 : r.body.length))}',
        );
        throw Exception('Response bukan JSON valid');
      }
    }
    debugPrint(
      '[API] ❌ _parse: HTTP ${r.statusCode} → ${r.body.substring(0, (r.body.length > 100 ? 100 : r.body.length))}',
    );
    throw Exception('HTTP ${r.statusCode}');
  }

  // ── Test koneksi ──────────────────────────────────────────────
  static Future<Map<String, dynamic>> testConnection(String serverUrl) async {
    try {
      String url = serverUrl.trim();
      if (!url.startsWith('http')) url = 'http://$url';
      final r = await _makeRequest(
        'get_stock.php?limit=1',
        overrideBaseUrl: url,
        customTimeout: const Duration(seconds: 8),
      );
      final d = _parse(r);
      if (d['status'] == 'success') {
        return {
          'status': 'success',
          'message': 'Koneksi berhasil',
          'server_url': url,
        };
      }
      return {'status': 'error', 'message': d['message'] ?? 'Server error'};
    } catch (e) {
      return {'status': 'error', 'message': 'Koneksi gagal: $e'};
    }
  }

  // ── Auth ──────────────────────────────────────────────────────
  static Future<Map<String, dynamic>> login(
    String username,
    String password,
  ) async {
    final r = await _makeRequest(
      'auth_login.php',
      method: 'POST',
      body: {'username': username, 'password': password},
    );
    return _parse(r);
  }

  // ── Users (dipanggil di login screen — sebelum ada token) ─────
  static Future<Map<String, dynamic>> getUsers() async {
    final r = await _makeRequest(
      'get_users.php',
      customTimeout: const Duration(seconds: 10),
    );
    return _parse(r);
  }

  // ── Pelanggan ─────────────────────────────────────────────────
  static Future<Map<String, dynamic>> getPelanggan() async {
    final r = await _makeRequest('get_pelanggan.php');
    return _parse(r);
  }

  // ── Databases ─────────────────────────────────────────────────
  static Future<Map<String, dynamic>> getDatabases() async {
    final r = await _makeRequest('get_databases.php');
    return _parse(r);
  }

  // ── Stok barang ───────────────────────────────────────────────
  static Future<Map<String, dynamic>> getStock({
    String search = '',
    int limit = 50,
    int offset = 0,
  }) async {
    final params = <String, String>{
      'limit': '$limit',
      'offset': '$offset',
      if (search.isNotEmpty) 'search': Uri.encodeQueryComponent(search),
    };
    final qs = params.entries.map((e) => '${e.key}=${e.value}').join('&');
    final r = await _makeRequest('get_stock.php?$qs');
    return _parse(r);
  }

  // ── Kategori & Merk ───────────────────────────────────────────
  static Future<Map<String, dynamic>> getKategori() async =>
      _parse(await _makeRequest('master_kategori_merk.php?type=kategori'));

  static Future<Map<String, dynamic>> getMerk() async =>
      _parse(await _makeRequest('master_kategori_merk.php?type=merk'));

  static Future<String> generateKodeKategori() async {
    final res = _parse(
      await _makeRequest(
        'master_kategori_merk.php?type=kategori&action=generate_kode',
      ),
    );
    return res['kode']?.toString() ?? '';
  }

  static Future<String> generateKodeMerk() async {
    final res = _parse(
      await _makeRequest(
        'master_kategori_merk.php?type=merk&action=generate_kode',
      ),
    );
    return res['kode']?.toString() ?? '';
  }

  static Future<Map<String, dynamic>> addKategori(
    String kode,
    String nama,
    String jenis,
  ) async => _parse(
    await _makeRequest(
      'master_kategori_merk.php?type=kategori',
      method: 'POST',
      body: {'kode': kode, 'nama': nama, 'jenis': jenis},
    ),
  );

  static Future<Map<String, dynamic>> updateKategori(
    String kode,
    String nama,
    String jenis,
  ) async => _parse(
    await _makeRequest(
      'master_kategori_merk.php?type=kategori&kode=$kode',
      method: 'PUT',
      body: {'nama': nama, 'jenis': jenis},
    ),
  );

  static Future<Map<String, dynamic>> deleteKategori(String kode) async =>
      _parse(
        await _makeRequest(
          'master_kategori_merk.php?type=kategori&kode=$kode',
          method: 'DELETE',
        ),
      );

  static Future<Map<String, dynamic>> addMerk(
    String kode,
    String nama,
    String ket,
  ) async => _parse(
    await _makeRequest(
      'master_kategori_merk.php?type=merk',
      method: 'POST',
      body: {'kode': kode, 'nama': nama, 'keterangan': ket},
    ),
  );

  static Future<Map<String, dynamic>> updateMerk(
    String kode,
    String nama,
    String ket,
  ) async => _parse(
    await _makeRequest(
      'master_kategori_merk.php?type=merk&kode=$kode',
      method: 'PUT',
      body: {'nama': nama, 'keterangan': ket},
    ),
  );

  static Future<Map<String, dynamic>> deleteMerk(String kode) async => _parse(
    await _makeRequest(
      'master_kategori_merk.php?type=merk&kode=$kode',
      method: 'DELETE',
    ),
  );

  static Future<Map<String, dynamic>> updateProductCategoryMerk(
    String idBarang,
    String? kategori,
    String? merk,
  ) async => _parse(
    await _makeRequest(
      'update_product.php',
      method: 'POST',
      body: {'id_barang': idBarang, 'kategori': kategori, 'merk': merk},
    ),
  );

  // ── Penjualan ─────────────────────────────────────────────────
  static Future<Map<String, dynamic>> syncPenjualan(
    Map<String, dynamic> data,
  ) async => _parse(
    await _makeRequest(
      'sync_penjualan.php',
      method: 'POST',
      body: data,
      customTimeout: const Duration(seconds: 15),
    ),
  );

  // ── Stok Opname ───────────────────────────────────────────────
  static Future<Map<String, dynamic>> getOpnameList({
    String lokasi = '',
    String tglDari = '',
    String tglSampai = '',
    int limit = 30,
    int offset = 0,
  }) async {
    final params = <String, String>{
      'limit': '$limit',
      'offset': '$offset',
      if (lokasi.isNotEmpty) 'lokasi': Uri.encodeQueryComponent(lokasi),
      if (tglDari.isNotEmpty) 'tgl_dari': Uri.encodeQueryComponent(tglDari),
      if (tglSampai.isNotEmpty)
        'tgl_sampai': Uri.encodeQueryComponent(tglSampai),
    };
    final qs = params.entries.map((e) => '${e.key}=${e.value}').join('&');
    return _parse(await _makeRequest('get_opname_list.php?$qs'));
  }

  static Future<Map<String, dynamic>> syncStokOpname(
    Map<String, dynamic> data,
  ) async => _parse(
    await _makeRequest(
      'sync_stokopname.php',
      method: 'POST',
      body: data,
      customTimeout: const Duration(seconds: 15),
    ),
  );

  // ── Transfer Stok ─────────────────────────────────────────────
  static Future<Map<String, dynamic>> getTransferList({
    String lokasi = '',
    String tglDari = '',
    String tglSampai = '',
    String search = '',
    int limit = 30,
    int offset = 0,
  }) async {
    final params = <String, String>{
      'limit': '$limit',
      'offset': '$offset',
      if (lokasi.isNotEmpty) 'lokasi': Uri.encodeQueryComponent(lokasi),
      if (tglDari.isNotEmpty) 'tgl_dari': Uri.encodeQueryComponent(tglDari),
      if (tglSampai.isNotEmpty)
        'tgl_sampai': Uri.encodeQueryComponent(tglSampai),
      if (search.isNotEmpty) 'search': Uri.encodeQueryComponent(search),
    };
    final qs = params.entries.map((e) => '${e.key}=${e.value}').join('&');
    return _parse(await _makeRequest('get_transfer_list.php?$qs'));
  }

  static Future<Map<String, dynamic>> syncTransferStok(
    Map<String, dynamic> data,
  ) async => _parse(
    await _makeRequest(
      'sync_transfer_stok.php',
      method: 'POST',
      body: data,
      customTimeout: const Duration(seconds: 15),
    ),
  );

  // ── Server Info (nama database) ──────────────────────────────
  static Future<Map<String, dynamic>> getServerInfo() async => _parse(
    await _makeRequest(
      'get_server_info.php',
      customTimeout: const Duration(seconds: 5),
    ),
  );

  // ── Data Perusahaan ───────────────────────────────────────────
  static Future<Map<String, dynamic>> getDataPerusahaan() async => _parse(
    await _makeRequest(
      'get_perusahaan.php',
      customTimeout: const Duration(seconds: 10),
    ),
  );

  // ── Hak Akses User ────────────────────────────────────────────
  static Future<Map<String, dynamic>> getHakAkses({
    Map<String, String>? roleMapping,
  }) async => _parse(
    await _makeRequest(
      'get_hak_akses.php',
      method: roleMapping != null ? 'POST' : 'GET',
      body: roleMapping != null ? {'role_mapping': roleMapping} : null,
      customTimeout: const Duration(seconds: 10),
    ),
  );

  // ── Dashboard Summary ─────────────────────────────────────────
  static Future<Map<String, dynamic>> getDashboardSummary({
    String lokasi = '',
  }) async {
    final qs = lokasi.isNotEmpty
        ? '?lokasi=${Uri.encodeQueryComponent(lokasi)}'
        : '';
    return _parse(
      await _makeRequest(
        'get_dashboard_summary.php$qs',
        customTimeout: const Duration(seconds: 8),
      ),
    );
  }

  // ── Karyawan (Sales) ──────────────────────────────────────────
  static Future<Map<String, dynamic>> getKaryawan() async =>
      _parse(await _makeRequest('get_karyawan.php'));

  // ── Akun COA ──────────────────────────────────────────────────
  static Future<Map<String, dynamic>> getAkunCOA({String tipe = ''}) async {
    final qs = tipe.isNotEmpty ? '?tipe=${Uri.encodeQueryComponent(tipe)}' : '';
    return _parse(await _makeRequest('get_akun_coa.php$qs'));
  }

  // ── Laporan Stok ──────────────────────────────────────────────
  static Future<Map<String, dynamic>> getLaporanStok({
    String search = '',
    String kategori = '',
    int limit = 50,
    int offset = 0,
  }) async {
    final params = <String, String>{
      'limit': '$limit',
      'offset': '$offset',
      if (search.isNotEmpty) 'search': Uri.encodeQueryComponent(search),
      if (kategori.isNotEmpty) 'kategori': Uri.encodeQueryComponent(kategori),
    };
    final qs = params.entries.map((e) => '${e.key}=${e.value}').join('&');
    return _parse(await _makeRequest('get_laporan_stok.php?$qs'));
  }

  // ── AI Analytics ─────────────────────────────────────────────
  static Future<Map<String, dynamic>> getAIAnalytics({
    required String type,
    String lokasi = '',
  }) async {
    final qs =
        'type=${Uri.encodeQueryComponent(type)}'
        '${lokasi.isNotEmpty ? '&lokasi=${Uri.encodeQueryComponent(lokasi)}' : ''}';
    // Timeout lebih lama untuk query berat (barang_lambat, reorder_alert)
    final heavyTypes = {'barang_lambat', 'reorder_alert', 'margin_profit'};
    final t = heavyTypes.contains(type)
        ? const Duration(seconds: 30)
        : const Duration(seconds: 15);
    return _parse(
      await _makeRequest('get_ai_analytics.php?$qs', customTimeout: t),
    );
  }

  // ── Riwayat Penjualan ─────────────────────────────────────────
  static Future<Map<String, dynamic>> getRiwayatPenjualan({
    String lokasi = '',
    String tglDari = '',
    String tglSampai = '',
    String search = '',
    int limit = 30,
    int offset = 0,
  }) async {
    final params = <String, String>{
      'mode': 'list',
      'limit': '$limit',
      'offset': '$offset',
      if (lokasi.isNotEmpty) 'lokasi': Uri.encodeQueryComponent(lokasi),
      if (tglDari.isNotEmpty) 'tgl_dari': Uri.encodeQueryComponent(tglDari),
      if (tglSampai.isNotEmpty)
        'tgl_sampai': Uri.encodeQueryComponent(tglSampai),
      if (search.isNotEmpty) 'search': Uri.encodeQueryComponent(search),
    };
    final qs = params.entries.map((e) => '${e.key}=${e.value}').join('&');
    return _parse(await _makeRequest('get_riwayat_penjualan.php?$qs'));
  }

  static Future<Map<String, dynamic>> getDetailPenjualan(
    String idPenjualan,
  ) async {
    final qs = 'mode=detail&faktur=${Uri.encodeQueryComponent(idPenjualan)}';
    return _parse(await _makeRequest('get_riwayat_penjualan.php?$qs'));
  }
}
