<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    echo json_encode(['status' => 'error', 'message' => 'Method not allowed']);
    exit();
}

$input = json_decode(file_get_contents('php://input'), true);

if (!$input) {
    echo json_encode(['status' => 'error', 'message' => 'Invalid input']);
    exit();
}

try {
    $host = $input['host'] ?? 'localhost';
    $port = $input['port'] ?? 3306;
    $username = $input['username'] ?? 'root';
    $password = $input['password'] ?? '';

    $dsn = "mysql:host=$host;port=$port;charset=utf8mb4";
    $conn = new PDO($dsn, $username, $password, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
        PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        PDO::ATTR_TIMEOUT => 5,
    ]);

    $stmt = $conn->query('SHOW DATABASES');
    $databases = $stmt->fetchAll(PDO::FETCH_COLUMN);

    $filteredDatabases = array_filter($databases, function($db) {
        return !in_array($db, ['information_schema', 'mysql', 'performance_schema', 'phpmyadmin', 'sys']);
    });

    $databasesWithTables = [];
    foreach ($filteredDatabases as $db) {
        $conn->exec("USE `$db`");
        $tablesStmt = $conn->query('SHOW TABLES');
        $tables = $tablesStmt->fetchAll(PDO::FETCH_COLUMN);
        
        $tablesWithColumns = [];
        foreach ($tables as $table) {
            $columnsStmt = $conn->query("SHOW COLUMNS FROM `$table`");
            $columns = $columnsStmt->fetchAll(PDO::FETCH_COLUMN);
            $tablesWithColumns[] = [
                'name' => $table,
                'columns' => $columns
            ];
        }
        
        $databasesWithTables[] = [
            'name' => $db,
            'tables' => $tablesWithColumns
        ];
    }

    echo json_encode([
        'status' => 'success',
        'message' => 'Database connection successful',
        'databases' => $databasesWithTables,
        'server_info' => $conn->getAttribute(PDO::ATTR_SERVER_INFO),
    ]);
} catch (PDOException $e) {
    echo json_encode([
        'status' => 'error',
        'message' => $e->getMessage(),
    ]);
}
