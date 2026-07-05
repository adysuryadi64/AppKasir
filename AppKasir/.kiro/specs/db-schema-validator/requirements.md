# Requirements Document

## Introduction

Sistem validasi skema database dan migrasi otomatis untuk AppKasir (VB.NET + MySQL).
Menggantikan pendekatan migrasi manual via file SQL (.sql) dengan pendekatan declarative:
developer mendefinisikan struktur tabel yang diharapkan di code, sistem membaca struktur aktual
dari database, membandingkan keduanya, dan generate SQL migration yang idempotent.
Sistem ini bertujuan mengurangi human error, memastikan konsistensi skema antar environment,
dan menyediakan audit trail untuk setiap perubahan struktur database.

## Glossary

- **SchemaRegistry**: Komponen VB.NET yang mendefinisikan struktur tabel yang diharapkan (expected schema) secara declarative dalam code
- **ExpectedSchema**: Struktur tabel ideal yang didefinisikan developer di SchemaRegistry
- **ActualSchema**: Struktur tabel aktual yang dibaca dari MySQL information_schema
- **SchemaDiff**: Hasil perbandingan antara ExpectedSchema dan ActualSchema
- **MigrationPlan**: Kumpulan SQL statement yang dihasilkan dari SchemaDiff untuk menyesuaikan ActualSchema ke ExpectedSchema
- **SchemaComparer**: Komponen yang melakukan perbandingan antara ExpectedSchema dan ActualSchema
- **MigrationGenerator**: Komponen yang menghasilkan SQL migration statement dari SchemaDiff
- **MigrationExecutor**: Komponen yang mengeksekusi MigrationPlan ke database
- **MigrationLog**: Catatan hasil eksekusi migrasi (waktu, status, statement, error)
- **FormSchemaValidator**: UI WinForms untuk menampilkan diff dan menjalankan migrasi
- **DbSchemaHelper**: Komponen helper untuk query information_schema MySQL

## Requirements

### Requirement 1: Schema Definition in Code

**User Story:** As a developer, I want to define expected database table structures in VB.NET code, so that the schema is version-controlled and lives alongside the application code.

#### Acceptance Criteria

1. THE SchemaRegistry SHALL provide a fluent API to define table name, columns (name, data type, nullable, default value, comment), primary key, indexes, and unique constraints
2. WHEN a developer defines a table schema via the SchemaRegistry API, THE SchemaRegistry SHALL store the definition as a TableSchema object containing all column definitions, index definitions, and constraint definitions
3. THE SchemaRegistry SHALL support MySQL data types including VARCHAR, CHAR, TEXT, TINYINT, SMALLINT, INT, BIGINT, DECIMAL, FLOAT, DOUBLE, DATE, DATETIME, TIMESTAMP, TINYINT(1) as BOOLEAN, and AUTO_INCREMENT on integer primary keys
4. THE SchemaRegistry SHALL support defining columns with NOT NULL constraint, DEFAULT values (including expressions like CURRENT_TIMESTAMP), and COMMENT strings
5. THE SchemaRegistry SHALL support defining single-column and multi-column indexes with optional UNIQUE constraint
6. THE SchemaRegistry SHALL support defining a table ENGINE (default InnoDB) and CHARSET (default utf8mb4 with utf8mb4_unicode_ci collation)

### Requirement 2: Read Actual Schema from Database

**User Story:** As a developer, I want the system to read the current database structure from information_schema, so that I can compare it against my expected schema.

#### Acceptance Criteria

1. WHEN the system queries information_schema.COLUMNS, THE DbSchemaHelper SHALL return all column definitions for each table including column name, data type, is_nullable, column_default, column_comment, and extra (for AUTO_INCREMENT)
2. WHEN the system queries information_schema.STATISTICS, THE DbSchemaHelper SHALL return all index definitions including index name, column name, is_unique, and seq_in_index
3. WHEN the system queries information_schema.TABLE_CONSTRAINTS, THE DbSchemaHelper SHALL return primary key and unique constraint definitions
4. WHEN the system queries information_schema.TABLES, THE DbSchemaHelper SHALL return table engine, table collation, and table comment
5. IF a table does not exist in the database, THEN THE DbSchemaHelper SHALL return an empty schema for that table name (not an error)
6. THE DbSchemaHelper SHALL use parameterized queries to prevent SQL injection when reading schema metadata

### Requirement 3: Schema Comparison

**User Story:** As a developer, I want the system to compare expected schema against actual database schema and identify all differences, so that I know exactly what changes are needed.

#### Acceptance Criteria

1. THE SchemaComparer SHALL detect tables that exist in ExpectedSchema but not in ActualSchema (missing tables)
2. THE SchemaComparer SHALL detect tables that exist in ActualSchema but not in ExpectedSchema (extra tables)
3. THE SchemaComparer SHALL detect columns that exist in ExpectedSchema but not in ActualSchema for each matching table (missing columns)
4. THE SchemaComparer SHALL detect columns that exist in ActualSchema but not in ExpectedSchema for each matching table (extra columns)
5. THE SchemaComparer SHALL detect column type mismatches between ExpectedSchema and ActualSchema including data type, length, nullable, and default value differences
6. THE SchemaComparer SHALL detect index differences including missing indexes, extra indexes, and index definition changes
7. THE SchemaComparer SHALL produce a structured SchemaDiff object containing categorized lists of all differences (missing tables, extra tables, column changes, index changes)
8. THE SchemaComparer SHALL perform case-insensitive comparison for table names and column names (MySQL convention)

### Requirement 4: Migration SQL Generation

**User Story:** As a developer, I want the system to generate idempotent SQL migration statements from the schema diff, so that migrations can be run safely multiple times without side effects.

#### Acceptance Criteria

1. THE MigrationGenerator SHALL generate CREATE TABLE IF NOT EXISTS statements for tables present in ExpectedSchema but missing from ActualSchema
2. THE MigrationGenerator SHALL generate ALTER TABLE statements for column additions (ADD COLUMN), type changes (MODIFY COLUMN), nullable changes, and default value changes
3. THE MigrationGenerator SHALL generate CREATE INDEX IF NOT EXISTS statements for missing indexes
4. THE MigrationGenerator SHALL generate ALTER TABLE ... DROP INDEX statements for extra indexes not present in ExpectedSchema
5. THE MigrationGenerator SHALL NOT generate DROP TABLE statements for tables present in ActualSchema but not in ExpectedSchema (safety: never drop tables automatically)
6. WHEN generating ALTER TABLE statements, THE MigrationGenerator SHALL combine multiple column changes for the same table into a single ALTER TABLE statement where possible to reduce execution time
7. THE MigrationGenerator SHALL produce SQL statements that are idempotent: running the same migration twice produces the same result with no errors
8. THE MigrationGenerator SHALL include a comment header in the generated SQL with timestamp and summary of changes

### Requirement 5: UI Schema Diff Viewer

**User Story:** As a developer, I want to see a visual diff of schema changes before applying them, so that I can review and approve each change.

#### Acceptance Criteria

1. WHEN the user triggers a schema check, THE FormSchemaValidator SHALL display a tree view grouped by table name showing all detected differences
2. WHEN displaying column differences, THE FormSchemaValidator SHALL show the column name, current type (in database), and expected type (from registry) side by side
3. WHEN displaying missing tables, THE FormSchemaValidator SHALL show the full CREATE TABLE statement that would be generated
4. WHERE the user selects a specific table in the diff view, THE FormSchemaValidator SHALL show detailed column-level and index-level differences for that table
5. WHEN the user clicks "Generate Migration", THE FormSchemaValidator SHALL show the complete SQL migration script in a read-only text area for review
6. WHEN the user clicks "Apply Migration", THE FormSchemaValidator SHALL prompt for confirmation before executing the migration
7. IF the schema has no differences, THEN THE FormSchemaValidator SHALL display a success message indicating the database is up to date

### Requirement 6: Migration Execution

**User Story:** As a developer, I want to apply the generated migration to the database, so that the actual schema matches the expected schema.

#### Acceptance Criteria

1. WHEN the user confirms migration execution, THE MigrationExecutor SHALL execute each SQL statement in the MigrationPlan sequentially
2. WHEN a SQL statement executes successfully, THE MigrationExecutor SHALL log the statement with status "SUCCESS" and continue to the next statement
3. IF a SQL statement fails with a non-critical error (column already exists, index already exists), THEN THE MigrationExecutor SHALL log it as "SKIPPED" and continue execution
4. IF a SQL statement fails with a critical error, THEN THE MigrationExecutor SHALL log it as "FAILED", stop execution, and display the error to the user
5. WHEN migration execution completes, THE MigrationExecutor SHALL display a summary with counts of successful, skipped, and failed statements
6. THE MigrationExecutor SHALL execute statements within a transaction where supported by MySQL (DDL statements in MySQL cause implicit commit, so each statement is executed individually with error handling)

### Requirement 7: Migration Logging

**User Story:** As a developer, I want migration results logged to a file, so that I can review past migrations and troubleshoot issues.

#### Acceptance Criteria

1. WHEN a migration is executed, THE MigrationExecutor SHALL write a log entry to a file named `migration_log_{date}.log` in the application directory
2. THE MigrationExecutor log entry SHALL include timestamp, database name, statement sequence number, SQL statement text, execution status, and error message (if any)
3. WHEN migration execution completes, THE MigrationExecutor SHALL append a summary line with total statements, successful count, skipped count, and failed count
4. THE MigrationExecutor SHALL preserve existing log files when creating new log entries (append mode, not overwrite)

### Requirement 8: Integration with Existing Migration System

**User Story:** As a developer, I want the new schema validator to work alongside the existing FormMigrasiDB manual migration system, so that both approaches remain available.

#### Acceptance Criteria

1. THE FormSchemaValidator SHALL be accessible from the FormUtama menu structure alongside the existing FormMigrasiDB
2. THE SchemaRegistry SHALL be independent of the existing SQL file-based migration system
3. WHERE manual SQL migrations have been applied that are not reflected in the SchemaRegistry, THE SchemaComparer SHALL detect these as "extra" items and NOT propose removing them
4. THE SchemaRegistry SHALL be able to incorporate definitions for tables created by manual SQL migrations by adding their definitions to the registry
