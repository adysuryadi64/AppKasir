# Design Document: DB Schema Validator

## Architecture Overview

Declarative schema validation system: developer defines expected table structures in SQL files under `Database/SchemaDef/`, system reads actual schema from MySQL `information_schema`, compares them, and generates idempotent migration SQL per table under `Database/SchemaGenerated/`.

### Flow
```
Database/SchemaDef/*.sql  →  SchemaRegistry (parse)  →  ExpectedSchema
                                                              ↓
DbSchemaHelper (information_schema)  →  ActualSchema   →  SchemaComparer  →  SchemaDiff
                                                                              ↓
                                                    MigrationGenerator  →  Database/SchemaGenerated/<table>.sql
                                                                              ↓
                                                    MigrationExecutor   →  MySQL + log file
                                                                              ↓
                                                    FormSchemaValidator  →  UI diff viewer + execute
```

## File Structure

```
AppKasir/
├── Database/
│   ├── SchemaDef/                      ← Declarative schema (one SQL per table)
│   │   ├── tbl_barang.sql
│   │   ├── tbl_pelanggan.sql
│   │   ├── tbl_supliyer.sql
│   │   ├── tbl_kategori.sql
│   │   ├── tbl_satuan.sql
│   │   ├── tbl_merk.sql
│   │   ├── tbl_armada.sql
│   │   ├── tbl_cabang.sql
│   │   ├── tbl_user.sql
│   │   ├── tbl_perusahaan.sql
│   │   ├── tbl_datareferensi.sql
│   │   ├── penjualan.sql
│   │   ├── penjualan_detail.sql
│   │   ├── pembelian.sql
│   │   ├── pembelian_detail.sql
│   │   ├── JurnalUmum.sql
│   │   ├── HistoryBarang.sql
│   │   ├── hakaksesuser.sql
│   │   ├── tbl_karyawan.sql
│   │   ├── tbl_audit_trail.sql
│   │   ├── tbl_audit_trail_arsip.sql
│   │   ├── sync_queue.sql
│   │   ├── sync_log.sql
│   │   ├── sync_config.sql
│   │   ├── sales_order.sql
│   │   ├── sales_order_detail.sql
│   │   ├── poin_config.sql
│   │   ├── poin_ledger.sql
│   │   ├── poin_barang.sql
│   │   ├── tbl_rakitan_bom.sql
│   │   ├── tbl_proses_rakitan.sql
│   │   ├── tbl_proses_rakitan_detail.sql
│   │   ├── transfer_cabang.sql
│   │   └── transfer_cabang_detail.sql
│   ├── SchemaGenerated/               ← Auto-generated migration SQL (per table)
│   │   ├── 0001_tbl_barang.sql
│   │   ├── 0002_penjualan.sql
│   │   └── ...
│   ├── 01_migrasi_kolom.sql           ← Existing manual migrations (preserved)
│   └── ... (02-51)
├── Modules/
│   ├── SchemaModels.vb                 ← Data classes: TableSchema, ColumnSchema, IndexSchema
│   ├── SchemaRegistry.vb              ← Parse SchemaDef/*.sql → ExpectedSchema
│   ├── DbSchemaHelper.vb              ← Query information_schema → ActualSchema
│   ├── SchemaComparer.vb              ← Compare → SchemaDiff
│   └── MigrationGenerator.vb          ← SchemaDiff → MigrationPlan + SQL files
├── 8Uty/
│   ├── FormSchemaValidator.vb         ← UI form
│   ├── FormSchemaValidator.Designer.vb
│   ├── FormSchemaValidator.resx
│   └── MigrationExecutor.vb           ← Execute migration + logging
```

## SchemaDef SQL File Format

Each file is a standard MySQL CREATE TABLE statement. Parsed by `SchemaRegistry` to extract metadata:

```sql
-- Table: tbl_barang
-- Deskripsi: Master data barang
CREATE TABLE IF NOT EXISTS `tbl_barang` (
    `ID_BARANG` varchar(15) NOT NULL,
    `NAMA_BARANG` varchar(200) NOT NULL DEFAULT '',
    `STOK_TOKO` decimal(15,2) NOT NULL DEFAULT 0,
    `STOK_GUDANG` decimal(15,2) NOT NULL DEFAULT 0,
    `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT 0,
    `HARGA_JUAL` decimal(15,2) NOT NULL DEFAULT 0,
    `HARGA_MEMBER` decimal(15,2) NOT NULL DEFAULT 0,
    `KODE_KATEGORI` varchar(10) DEFAULT NULL,
    `KODE_SATUAN` varchar(10) DEFAULT NULL,
    `KODE_MERK` varchar(10) DEFAULT NULL,
    `SERIAL_NUMBER` varchar(50) DEFAULT NULL,
    `ID_USER` varchar(20) DEFAULT NULL,
    `id_cloud` varchar(50) DEFAULT NULL,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `updated_by` varchar(50) DEFAULT NULL,
    `is_dirty` tinyint NOT NULL DEFAULT 1,
    `version` int NOT NULL DEFAULT 1,
    PRIMARY KEY (`ID_BARANG`),
    KEY `idx_kategori` (`KODE_KATEGORI`),
    KEY `idx_merk` (`KODE_MERK`),
    KEY `idx_is_dirty` (`is_dirty`),
    KEY `idx_id_cloud` (`id_cloud`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Master data barang';
```

## Key Components

### SchemaModels.vb
Data classes: `TableSchema`, `ColumnSchema`, `IndexSchema`, `SchemaDiff`, `MigrationPlan`, `MigrationStep`.

### SchemaRegistry.vb
- `LoadFromFolder(schemaDir As String)` → reads all `*.sql` files from SchemaDef/
- Parses CREATE TABLE statements using regex
- Returns `Dictionary(Of String, TableSchema)` keyed by table name
- Supports `GetTableSchema(tableName)` for single table lookup

### DbSchemaHelper.vb
- `GetActualSchema(conn, tableName)` → reads from `information_schema.COLUMNS`, `STATISTICS`, `TABLE_CONSTRAINTS`, `TABLES`
- Uses parameterized queries
- Returns `TableSchema` (same model as registry)

### SchemaComparer.vb
- `Compare(expected, actual)` → `SchemaDiff`
- Detects: missing tables, extra tables, column diffs, index diffs
- Case-insensitive comparison (MySQL convention)
- Extra items from actual DB reported as "info" only — never proposed for removal

### MigrationGenerator.vb
- `Generate(diff)` → `MigrationPlan` (list of SQL statements per table)
- `SaveToFolder(plan, outputDir)` → writes `Database/SchemaGenerated/NNNN_<table>.sql`
- Idempotent SQL: `CREATE TABLE IF NOT EXISTS`, `ADD COLUMN IF NOT EXISTS` (MySQL 8+), or IF-exists checks
- One SQL file per table in the diff

### MigrationExecutor.vb
- `Execute(plan, conn)` → runs statements sequentially
- Logs to `migration_log_{date}.log`
- Handles MySQL errors: 1060 (dup col), 1061 (dup key), 1091 (can't drop) → SKIPPED
- Other errors → FAILED + stop
- Returns summary: success/skipped/failed counts

### FormSchemaValidator.vb
- TreeView: tables grouped by status (missing/column changes/index changes)
- Detail panel: column-level diff (current vs expected side-by-side)
- Buttons: "Check Schema" → "Generate Migration" → "Apply Migration"
- Shows generated SQL in read-only TextBox before applying
- Confirmation dialog before executing
- Accessible from FormUtama menu alongside FormMigrasiDB

## Design Decisions

1. **SQL files over VB.NET code**: Schema definitions live in plain SQL — easier to review, diff, and maintain than VB.NET fluent API
2. **One file per table**: Keeps each file small and focused. MigrationGenerator also outputs one file per table.
3. **Coexistence with existing migrations**: Old `01-51_migrasi_*.sql` files preserved. SchemaValidator compares current DB state against SchemaDef, so old manual migrations are accounted for.
4. **Never auto-drop**: Extra tables/columns in DB but not in SchemaDef are reported as info, never dropped automatically.
5. **Idempotent SQL**: All generated migration SQL can be run multiple times safely.
