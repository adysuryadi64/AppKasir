<?php
// Blokir akses langsung ke file ini via browser
// config.php hanya boleh di-include oleh PHP, tidak diakses sebagai URL
if (basename($_SERVER['SCRIPT_FILENAME']) === basename(__FILE__)) {
    http_response_code(403);
    exit('Forbidden');
}

return [
    'host'    => 'localhost',
    'db_name' => 'db_kasirlancar',
    'username' => 'root',
    'password' => '12345678',
    'port'    => 3306,
    'charset' => 'utf8mb4',
];
