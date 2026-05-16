<?php
// db_connect.php
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");
header("Content-Type: application/json; charset=UTF-8");

// Load configuration from config.php (absolute path using __DIR__)
$configPath = __DIR__ . '/config.php';
$config = file_exists($configPath) ? include $configPath : [
    'host' => 'localhost',
    'db_name' => 'db_kasir',
    'username' => 'root',
    'password' => '',
    'port' => 3306,
    'charset' => 'utf8mb4',
];

try {
    $dsn = "mysql:host={$config['host']};port={$config['port']};dbname={$config['db_name']};charset={$config['charset']}";
    $conn = new PDO($dsn, $config['username'], $config['password']);
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch(PDOException $exception) {
    echo json_encode(array("status" => "error", "message" => "Connection error: " . $exception->getMessage()));
    exit();
}
?>
