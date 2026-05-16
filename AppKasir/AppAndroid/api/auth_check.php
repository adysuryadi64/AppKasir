<?php
/**
 * auth_check.php — Validasi Bearer token
 * Jika diakses dari browser (bukan Flutter), tampilkan halaman 403 HTML.
 */

function isBrowserRequest(): bool {
    $accept = $_SERVER['HTTP_ACCEPT'] ?? '';
    $ua     = $_SERVER['HTTP_USER_AGENT'] ?? '';
    // Browser selalu mengirim Accept: text/html
    // Flutter/http tidak mengirim Accept: text/html
    return str_contains($accept, 'text/html');
}

function denyWithPage(): void {
    http_response_code(403);
    $file = __DIR__ . '/403.html';
    if (file_exists($file)) {
        // Ganti tag PHP date di 403.html dengan tahun sekarang
        $html = file_get_contents($file);
        $html = str_replace('<?php echo date(\'Y\'); ?>', date('Y'), $html);
        header('Content-Type: text/html; charset=UTF-8');
        echo $html;
    } else {
        header('Content-Type: text/html; charset=UTF-8');
        echo '<h1>403 Forbidden</h1>';
    }
    exit();
}

function requireAuth(PDO $conn): array {
    // Jika dari browser — tampilkan halaman 403 HTML
    if (isBrowserRequest()) {
        denyWithPage();
    }

    // Apache kadang tidak meneruskan Authorization ke $_SERVER['HTTP_AUTHORIZATION'].
    // Coba beberapa sumber sekaligus — urutan dari yang paling umum.
    $header = $_SERVER['HTTP_AUTHORIZATION']
           ?? $_SERVER['REDIRECT_HTTP_AUTHORIZATION']
           ?? (function_exists('apache_request_headers')
               ? (apache_request_headers()['Authorization']
                  ?? apache_request_headers()['authorization']
                  ?? '')
               : '');

    if (empty($header) || strpos($header, 'Bearer ') !== 0) {
        http_response_code(401);
        header('Content-Type: application/json');
        echo json_encode(['status' => 'error', 'message' => 'Unauthorized — token tidak ditemukan']);
        exit();
    }

    $token = substr($header, 7);

    if (empty($token)) {
        http_response_code(401);
        header('Content-Type: application/json');
        echo json_encode(['status' => 'error', 'message' => 'Unauthorized — token kosong']);
        exit();
    }

    try {
        $stmt = $conn->prepare(
            "SELECT KODE_USER, NAMA_USER, USER_NAME, LVL
             FROM tbl_user
             WHERE login_session_key = :token AND status = 'Aktif'
             LIMIT 1"
        );
        $stmt->execute([':token' => $token]);

        if ($stmt->rowCount() === 0) {
            http_response_code(401);
            header('Content-Type: application/json');
            echo json_encode(['status' => 'error', 'message' => 'Unauthorized — sesi tidak valid']);
            exit();
        }

        return $stmt->fetch(PDO::FETCH_ASSOC);

    } catch (Exception $e) {
        http_response_code(500);
        header('Content-Type: application/json');
        echo json_encode(['status' => 'error', 'message' => 'Gagal validasi token']);
        exit();
    }
}
?>
