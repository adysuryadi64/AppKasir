<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

function _log(string $tag, string $msg): void { error_log("[AuthLogin][$tag] $msg"); }

$data = json_decode(file_get_contents('php://input'));

if (!isset($data->username) || !isset($data->password)) {
    http_response_code(400);
    echo json_encode(['status' => 'error', 'message' => 'Username dan Password wajib diisi']);
    exit();
}

$username      = $data->username;
$password_hash = md5($data->password); // MD5 sesuai desktop VB.NET

try {
    $stmt = $conn->prepare(
        "SELECT KODE_USER, NAMA_USER, USER_NAME, LVL
         FROM tbl_user
         WHERE USER_NAME = :u AND PWD = :p AND status = 'Aktif'
         LIMIT 1"
    );
    $stmt->execute([':u' => $username, ':p' => $password_hash]);

    if ($stmt->rowCount() === 0) {
        http_response_code(401);
        echo json_encode(['status' => 'error', 'message' => 'Username atau Password salah, atau akun tidak aktif']);
        exit();
    }

    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    // Generate session token — disimpan di kolom login_session_key
    // Token: hash dari kode_user + timestamp + random bytes
    $token = bin2hex(random_bytes(24)); // 48 karakter hex, unik per login

    $conn->prepare(
        "UPDATE tbl_user SET login_session_key = :token WHERE KODE_USER = :kode"
    )->execute([':token' => $token, ':kode' => $row['KODE_USER']]);

    echo json_encode([
        'status'  => 'success',
        'message' => 'Login Berhasil',
        'token'   => $token,
        'data'    => [
            'KODE_USER' => $row['KODE_USER'],
            'NAMA_USER' => $row['NAMA_USER'],
            'USER_NAME' => $row['USER_NAME'],
            'LVL'       => $row['LVL'],
        ],
        'lokasi_options' => ['TOKO', 'GUDANG'],
    ]);

} catch (Exception $e) {
    error_log("[AuthLogin][EXCEPTION] " . $e->getMessage());
    http_response_code(500);
    echo json_encode(['status' => 'error', 'message' => 'Terjadi kesalahan sistem']);
}
?>
