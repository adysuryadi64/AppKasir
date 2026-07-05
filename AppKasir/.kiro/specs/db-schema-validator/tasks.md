# Implementation Plan: DB Schema Validator

## Task 1: Create SchemaModels.vb
- [ ] 1.1 Create `Modules/SchemaModels.vb` with data classes:
  - `ColumnSchema`: Name, DataType, Length, Precision, Scale, IsNullable, DefaultValue, IsAutoIncrement, Comment
  - `IndexSchema`: Name, Columns (List(Of String)), IsUnique
  - `TableSchema`: TableName, Columns (List(Of ColumnSchema)), Indexes (List(Of IndexSchema)), PrimaryKey (List(Of String)), Engine, Charset, Collation, Comment
  - `SchemaDiff`: MissingTables, ExtraTables, ColumnChanges (per table), IndexChanges (per table)
  - `ColumnChange`: TableName, ColumnName, ChangeType (Add/Modify/Drop), Expected, Actual
  - `IndexChange`: TableName, IndexName, ChangeType (Add/Drop), ExpectedIndex, ActualIndex
  - `MigrationStep`: TableName, SqlStatements (List(Of String)), Description
  - `MigrationPlan`: Steps (List(Of MigrationStep)), GeneratedAt, Summary

## Task 2: Create SchemaRegistry.vb
- [ ] 2.1 Create `Modules/SchemaRegistry.vb`
- [ ] 2.2 Implement `LoadFromFolder(schemaDir As String)` — reads all `*.sql` files
- [ ] 2.3 Implement CREATE TABLE SQL parser using regex:
  - Extract table name, columns (name, type, nullable, default, auto_increment, comment)
  - Extract PRIMARY KEY, INDEX, UNIQUE KEY definitions
  - Extract ENGINE, DEFAULT CHARSET, COLLATE, COMMENT
- [ ] 2.4 Implement `GetTableSchema(tableName)` lookup
- [ ] 2.5 Handle edge cases: backtick-quoted names, inline comments, multi-line definitions

## Task 3: Create DbSchemaHelper.vb
- [ ] 3.1 Create `Modules/DbSchemaHelper.vb`
- [ ] 3.2 Implement `GetActualTableSchema(conn, tableName)`:
  - Query `information_schema.COLUMNS` for column definitions
  - Query `information_schema.STATISTICS` for index definitions
  - Query `information_schema.TABLE_CONSTRAINTS` for primary key
  - Query `information_schema.TABLES` for engine, collation, comment
- [ ] 3.3 Use parameterized queries throughout
- [ ] 3.4 Return `TableSchema` (same model as registry) or Nothing if table doesn't exist

## Task 4: Create SchemaComparer.vb
- [ ] 4.1 Create `Modules/SchemaComparer.vb`
- [ ] 4.2 Implement `Compare(expected As Dictionary(Of String, TableSchema), actual As Dictionary(Of String, TableSchema))` → SchemaDiff
- [ ] 4.3 Detect missing tables (in expected but not in actual)
- [ ] 4.4 Detect extra tables (in actual but not in expected) — info only, no removal proposed
- [ ] 4.5 Per-table column comparison: detect add/modify/drop columns
- [ ] 4.6 Per-table index comparison: detect add/drop indexes
- [ ] 4.7 Case-insensitive comparison for table and column names

## Task 5: Create MigrationGenerator.vb
- [ ] 5.1 Create `Modules/MigrationGenerator.vb`
- [ ] 5.2 Implement `Generate(diff As SchemaDiff)` → MigrationPlan
- [ ] 5.3 Generate `CREATE TABLE IF NOT EXISTS` for missing tables
- [ ] 5.4 Generate `ALTER TABLE ... ADD COLUMN` for missing columns (with IF NOT EXISTS pattern)
- [ ] 5.5 Generate `ALTER TABLE ... MODIFY COLUMN` for type/nullable/default changes
- [ ] 5.6 Generate `CREATE INDEX IF NOT EXISTS` for missing indexes
- [ ] 5.7 Generate `ALTER TABLE ... DROP INDEX` for extra indexes
- [ ] 5.8 Combine multiple column changes per table into single ALTER TABLE
- [ ] 5.9 Implement `SaveToFolder(plan, outputDir)` — writes `SchemaGenerated/NNNN_<table>.sql`
- [ ] 5.10 Include comment header with timestamp and change summary

## Task 6: Create MigrationExecutor.vb
- [ ] 6.1 Create `8Uty/MigrationExecutor.vb`
- [ ] 6.2 Implement `Execute(plan As MigrationPlan, conn As MySqlConnection)` — runs statements sequentially
- [ ] 6.3 Handle MySQL error codes: 1060/1061/1091 → SKIPPED, others → FAILED + stop
- [ ] 6.4 Implement logging to `migration_log_{date}.log`
- [ ] 6.5 Return execution summary (success/skipped/failed counts)

## Task 7: Create SchemaDef SQL Files
- [ ] 7.1 Create `Database/SchemaDef/` folder
- [ ] 7.2 Create SQL files for core tables from existing migration files:
  - `tbl_barang.sql`, `tbl_pelanggan.sql`, `tbl_supliyer.sql`
  - `tbl_kategori.sql`, `tbl_satuan.sql`, `tbl_merk.sql`
  - `tbl_armada.sql`, `tbl_cabang.sql`, `tbl_user.sql`
  - `tbl_perusahaan.sql`, `tbl_datareferensi.sql`
  - `tbl_karyawan.sql`, `hakaksesuser.sql`
- [ ] 7.3 Create SQL files for transaction tables:
  - `penjualan.sql`, `penjualan_detail.sql`
  - `pembelian.sql`, `pembelian_detail.sql`
  - `JurnalUmum.sql`, `HistoryBarang.sql`
- [ ] 7.4 Create SQL files for sync/audit tables:
  - `sync_queue.sql`, `sync_log.sql`, `sync_config.sql`
  - `tbl_audit_trail.sql`, `tbl_audit_trail_arsip.sql`
- [ ] 7.5 Create SQL files for feature tables:
  - `sales_order.sql`, `sales_order_detail.sql`
  - `poin_config.sql`, `poin_ledger.sql`, `poin_barang.sql`
  - `tbl_rakitan_bom.sql`, `tbl_proses_rakitan.sql`, `tbl_proses_rakitan_detail.sql`
  - `transfer_cabang.sql`, `transfer_cabang_detail.sql`

## Task 8: Create FormSchemaValidator UI
- [ ] 8.1 Create `8Uty/FormSchemaValidator.vb`, `.Designer.vb`, `.resx`
- [ ] 8.2 Design layout: TreeView (left), detail panel (right), SQL preview (bottom)
- [ ] 8.3 Implement "Check Schema" button — loads registry + reads actual schema + compares
- [ ] 8.4 Implement TreeView population from SchemaDiff
- [ ] 8.5 Implement detail panel: column diff side-by-side view
- [ ] 8.6 Implement "Generate Migration" button — generates SQL + shows in TextBox
- [ ] 8.7 Implement "Apply Migration" button — confirmation dialog + execute
- [ ] 8.8 Apply existing theme via `ModuleTheme.TerapkanTheme(Me)`

## Task 9: Integration with FormUtama
- [ ] 9.1 Add menu item in FormUtama alongside existing FormMigrasiDB
- [ ] 9.2 Wire up click handler to open FormSchemaValidator

## Task 10: Testing
- [ ] 10.1 Test SchemaRegistry parses all SchemaDef SQL files correctly
- [ ] 10.2 Test DbSchemaHelper reads actual schema from database
- [ ] 10.3 Test SchemaComparer detects missing tables, columns, indexes
- [ ] 10.4 Test MigrationGenerator produces valid idempotent SQL
- [ ] 10.5 Test MigrationExecutor runs SQL and handles errors
- [ ] 10.6 Test FormSchemaValidator UI end-to-end flow
