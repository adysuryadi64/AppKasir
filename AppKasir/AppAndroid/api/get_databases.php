<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, Authorization');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}

require_once 'db_connect.php';
// get_databases hanya dipanggil dari admin panel (index.php via fetch POST)
// bukan dari browser langsung — tidak perlu no_browser karena dipanggil via JS fetch
$systemDatabases = ['information_schema', 'mysql', 'performance_schema', 'sys', 'phpmyadmin'];

try {
    $stmt = $conn->query("SHOW DATABASES");
    $databases = [];

    while ($row = $stmt->fetch(PDO::FETCH_NUM)) {
        $name = $row[0];
        if (!in_array(strtolower($name), $systemDatabases)) {
            $databases[] = ['name' => $name];
        }
    }

    echo json_encode([
        'status'  => 'success',
        'message' => 'Databases retrieved successfully',
        'data'    => $databases,
    ]);

} catch (Exception $e) {
    echo json_encode([
        'status'  => 'error',
        'message' => 'Failed to retrieve databases: ' . $e->getMessage(),
    ]);
}
?>
