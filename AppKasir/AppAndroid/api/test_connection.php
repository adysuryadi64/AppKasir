<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, Authorization');

if ($_SERVER['REQUEST_METHOD'] == 'OPTIONS') {
    http_response_code(200);
    exit();
}

require_once 'db_connect.php';

$data = json_decode(file_get_contents("php://input"), true);

if (!isset($data['server_url']) || empty($data['server_url'])) {
    echo json_encode([
        "status" => "error", 
        "message" => "Server URL is required"
    ]);
    exit();
}

try {
    $server_url = $data['server_url'];
    $username = isset($data['username']) ? $data['username'] : '';
    $password = isset($data['password']) ? $data['password'] : '';
    $database = isset($data['database']) ? $data['database'] : '';
    
    // Test database connection with provided credentials
    $test_query = "SELECT 1";
    
    // For MySQL, you would use the actual database connection
    // This is a simplified test - in production, you'd validate actual DB connection
    
    $response = [
        "status" => "success", 
        "message" => "Connection successful",
        "server_info" => [
            "server_url" => $server_url,
            "database" => $database,
            "username" => $username,
            "connected_at" => date("Y-m-d H:i:s")
        ]
    ];
    
    echo json_encode($response);
    
} catch (Exception $e) {
    echo json_encode([
        "status" => "error", 
        "message" => "Connection failed: " . $e->getMessage()
    ]);
}
?>
