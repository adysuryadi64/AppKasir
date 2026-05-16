<?php
// Test config.php dan password
header("Content-Type: text/plain; charset=UTF-8");

echo "=== TEST CONFIG.PHP ===" . PHP_EOL;
$configPath = __DIR__ . '/config.php';
echo "Config path: " . $configPath . PHP_EOL;
echo "File exists? " . (file_exists($configPath) ? "YES" : "NO") . PHP_EOL;
echo "File readable? " . (is_readable($configPath) ? "YES" : "NO") . PHP_EOL;
echo PHP_EOL;

if (file_exists($configPath)) {
    $config = include $configPath;
    echo "=== KONTEN CONFIG.PHP (disembunyikan password penuh) ===" . PHP_EOL;
    echo "Host: " . $config['host'] . PHP_EOL;
    echo "DB Name: " . $config['db_name'] . PHP_EOL;
    echo "Username: " . $config['username'] . PHP_EOL;
    echo "Password Length: " . strlen($config['password']) . PHP_EOL;
    echo "Password Preview: " . (strlen($config['password']) > 0 
        ? substr($config['password'], 0, 2) . str_repeat('*', max(0, strlen($config['password']) - 2))
        : '(KOSONG!)') . PHP_EOL;
    echo "Port: " . $config['port'] . PHP_EOL;
    echo PHP_EOL;

    echo "=== TEST KONEKSI DENGAN CONFIG.PHP ===" . PHP_EOL;
    try {
        $dsn = "mysql:host={$config['host']};port={$config['port']};dbname={$config['db_name']};charset={$config['charset']}";
        $conn = new PDO($dsn, $config['username'], $config['password']);
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        echo "✅ KONEKSI BERHASIL!" . PHP_EOL;
    } catch (PDOException $e) {
        echo "❌ KONEKSI GAGAL!" . PHP_EOL;
        echo "Error: " . $e->getMessage() . PHP_EOL;
    }
}
?>