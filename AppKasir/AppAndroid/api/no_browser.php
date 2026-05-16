<?php
/**
 * no_browser.php — Blokir akses dari browser, izinkan dari Flutter/HTTP client
 * Pakai di endpoint yang tidak butuh token tapi tidak boleh dibuka browser.
 *
 * Cara pakai:
 *   require_once 'no_browser.php';
 */

$accept = $_SERVER['HTTP_ACCEPT'] ?? '';
if (str_contains($accept, 'text/html')) {
    http_response_code(403);
    $file = __DIR__ . '/403.html';
    if (file_exists($file)) {
        header('Content-Type: text/html; charset=UTF-8');
        echo file_get_contents($file);
    }
    exit();
}
?>
