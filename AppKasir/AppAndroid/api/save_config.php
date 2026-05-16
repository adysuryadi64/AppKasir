<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST');
header('Access-Control-Allow-Headers: Content-Type, X-Admin-Token');

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    echo json_encode(['status' => 'error', 'message' => 'Method not allowed']);
    exit();
}

// Token sederhana — ganti nilai ini, jangan biarkan default
// Harus cocok dengan yang dikirim dari index.php saat save
define('ADMIN_TOKEN', 'kasir-admin-2026');

$token = $_SERVER['HTTP_X_ADMIN_TOKEN'] ?? '';
if ($token !== ADMIN_TOKEN) {
    http_response_code(403);
    echo json_encode(['status' => 'error', 'message' => 'Unauthorized']);
    exit();
}

$input = json_decode(file_get_contents('php://input'), true);

if (!$input) {
    echo json_encode(['status' => 'error', 'message' => 'Invalid input']);
    exit();
}

$config = [
    'host'     => $input['host']     ?? 'localhost',
    'db_name'  => $input['db_name']  ?? 'db_kasirlancar',
    'username' => $input['username'] ?? 'root',
    'password' => $input['password'] ?? '',
    'port'     => (int)($input['port'] ?? 3306),
    'charset'  => 'utf8mb4',
];

// Blokir akses langsung ke config.php yang akan ditulis
$configContent = "<?php\n"
    . "if (basename(\$_SERVER['SCRIPT_FILENAME']) === basename(__FILE__)) {\n"
    . "    http_response_code(403); exit('Forbidden');\n"
    . "}\n"
    . "return " . var_export($config, true) . ";\n";

try {
    if (file_put_contents('config.php', $configContent) !== false) {
        echo json_encode(['status' => 'success', 'message' => 'Konfigurasi berhasil disimpan']);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Gagal menulis file config']);
    }
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
