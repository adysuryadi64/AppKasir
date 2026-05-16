<!DOCTYPE html>
<html lang="id">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Kasir Lancar · Server Config</title>
<link rel="icon" type="image/x-icon" href="favicon.ico">
<style>
:root {
  --bg:       #0a0c14;
  --surface:  #141824;
  --border:   #2a2f3e;
  --accent:   #6366f1;
  --accent2:  #818cf8;
  --success:  #10b981;
  --danger:   #ef4444;
  --text:     #e8ecf4;
  --muted:    #6b7280;
  --input-bg: #0f1219;
  --radius:   14px;
  --font: 'Inter', system-ui, -apple-system, sans-serif;
  --shadow-lg: 0 25px 60px rgba(0,0,0,0.6), 0 0 100px rgba(99,102,241,0.08);
}
*{margin:0;padding:0;box-sizing:border-box}

/* Base font scale — optimized for desktop */
html{ font-size: 16px }

body{
  background:var(--bg);
  color:var(--text);
  font-family:var(--font);
  min-height:100vh;
  display:flex;
  align-items:center;
  justify-content:center;
  padding: 40px 24px;
  position: relative;
  overflow-x: hidden;
}

/* Canvas background akan menempati seluruh layar */
#starCanvas {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: block;
  z-index: -2;
  pointer-events: none;
}

/* Card dengan efek glassmorphism agar background terlihat */
.card{
  width:100%;
  max-width: 820px;
  background:rgba(20, 24, 36, 0.82);
  backdrop-filter: blur(16px);
  border:1px solid var(--border);
  border-radius:24px;
  overflow:hidden;
  box-shadow: var(--shadow-lg);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  z-index: 1;
}
.card:hover{
  box-shadow: 0 30px 70px rgba(0,0,0,0.7), 0 0 120px rgba(99,102,241,0.12);
}

/* ── Header ── */
.header{
  padding: 32px 40px;
  border-bottom:1px solid var(--border);
  display:flex;
  align-items:center;
  gap: 24px;
  background: linear-gradient(135deg, rgba(99,102,241,0.06) 0%, transparent 100%);
}
.logo{
  width: 72px;
  height: 72px;
  background:transparent;
  border-radius:16px;
  display:flex;align-items:center;justify-content:center;
  flex-shrink:0;
  box-shadow: 0 4px 12px rgba(0,0,0,0.3);
}
.logo img{
  width:100%;height:100%;
  object-fit:contain;border-radius:14px;
}
.header-text{flex:1;min-width:0}
.header-text h1{
  font-size: 26px;
  font-weight:800;letter-spacing:-.5px;
  background: linear-gradient(135deg, #e8ecf4 0%, #818cf8 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
.header-text p{
  font-size: 15px;
  color:var(--muted);margin-top:6px;
  font-weight:500;
}
.header-badges{
  display:flex;flex-direction:column;align-items:flex-end;gap:8px;
  flex-shrink:0;
}
.badge{
  display:inline-flex;align-items:center;gap:6px;
  font-size: 13px;
  font-weight:600;letter-spacing:.3px;
  padding:6px 14px;border-radius:24px;white-space:nowrap;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
}
.badge-android{background:rgba(16,185,129,.15);color:#34d399;border:1px solid rgba(16,185,129,.3)}
.badge-wifi{background:rgba(99,102,241,.15);color:var(--accent2);border:1px solid rgba(99,102,241,.3)}
.badge svg{width:13px;height:13px;fill:currentColor;flex-shrink:0}

/* ── Body ── */
.body{ padding: 36px 40px }

/* ── Section label ── */
.section-label{
  font-size: 13px;
  font-weight:700;
  letter-spacing:1px;
  text-transform:uppercase;
  color:var(--muted);
  margin-bottom:18px;
  display: flex;
  align-items: center;
  gap: 10px;
}
.section-label::after{
  content: '';
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, var(--border) 0%, transparent 100%);
}

/* ── Grid ── */
.grid-2{display:grid;grid-template-columns:1fr 1fr;gap:16px}
.grid-1{display:grid;grid-template-columns:1fr;gap:16px}

/* ── Field ── */
.field{display:flex;flex-direction:column;gap:8px}
.field label{
  font-size: 14px;
  color:var(--muted);font-weight:600;
}
.field-wrap{position:relative}
.field input{
  width:100%;
  background:rgba(15, 18, 25, 0.9);
  border:1.5px solid var(--border);
  border-radius:var(--radius);
  color:var(--text);
  font-size: 15px;
  font-family:var(--font);
  padding: 14px 18px;
  outline:none;
  transition:all 0.2s ease;
}
.field input:hover{
  border-color: rgba(99,102,241,0.4);
}
.field input:focus{
  border-color:var(--accent);
  box-shadow:0 0 0 4px rgba(99,102,241,0.12);
  background: rgba(15, 18, 25, 1);
}
.field input::placeholder{color:var(--muted)}
.eye-btn{
  position:absolute;right:14px;top:50%;transform:translateY(-50%);
  background:none;border:none;cursor:pointer;
  color:var(--muted);display:flex;align-items:center;
  padding:6px;border-radius:8px;
  transition: all 0.2s ease;
}
.eye-btn:hover{color:var(--text);background:rgba(99,102,241,0.1)}
.eye-btn svg{width:20px;height:20px}

/* ── Divider ── */
.divider{height:1px;background:var(--border);margin:20px 0}

/* ── Buttons ── */
.btn{
  display:inline-flex;align-items:center;justify-content:center;gap:10px;
  border:none;border-radius:var(--radius);
  font-size: 15px;
  font-weight:700;font-family:var(--font);
  cursor:pointer;transition:all 0.2s ease;
  padding: 15px 24px;
  white-space:nowrap;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
}
.btn:disabled{opacity:.4;cursor:not-allowed}
.btn-primary{background:var(--accent);color:#fff}
.btn-primary:hover:not(:disabled){
  background:var(--accent2);
  transform:translateY(-2px);
  box-shadow: 0 6px 20px rgba(99,102,241,0.35);
}
.btn-success{background:var(--success);color:#fff}
.btn-success:hover:not(:disabled){
  filter:brightness(1.1);
  transform:translateY(-2px);
  box-shadow: 0 6px 20px rgba(16,185,129,0.35);
}
.btn-ghost{background:transparent;color:var(--muted);border:1.5px solid var(--border);box-shadow: none}
.btn-ghost:hover:not(:disabled){color:var(--text);border-color:var(--muted);background:rgba(99,102,241,0.05)}
.btn-full{width:100%}
.btn-row{display:flex;gap:12px;margin-top:20px}

/* ── Status ── */
.status{
  display:none;
  font-size: 15px;
  font-weight:600;
  padding:14px 18px;
  border-radius:var(--radius);
  margin-bottom:20px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
}
.status-success{background:rgba(16,185,129,.15);color:#34d399;border:1px solid rgba(16,185,129,.3)}
.status-error{background:rgba(239,68,68,.15);color:#f87171;border:1px solid rgba(239,68,68,.3)}

/* ── DB List ── */
.db-section{display:none;margin-top:24px}
.db-grid{
  display:grid;
  grid-template-columns:repeat(4, 1fr);
  gap:12px;
}
.db-item{
  display:flex;align-items:center;gap:12px;
  background:rgba(15,18,25,0.8);
  border:1.5px solid var(--border);
  border-radius:var(--radius);
  padding:14px 16px;
  cursor:pointer;
  transition:all 0.2s ease;
}
.db-item:hover{
  border-color:var(--accent);
  background:rgba(99,102,241,0.12);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(99,102,241,0.2);
}
.db-item.selected{
  border-color:var(--accent);
  background:rgba(99,102,241,0.18);
  box-shadow: 0 4px 16px rgba(99,102,241,0.25);
}
.db-icon{
  width:34px;height:34px;flex-shrink:0;
  background:rgba(99,102,241,.15);
  border-radius:10px;
  display:flex;align-items:center;justify-content:center;
  transition: all 0.2s ease;
}
.db-icon svg{width:17px;height:17px;fill:var(--accent2)}
.db-item.selected .db-icon{background:var(--accent)}
.db-item.selected .db-icon svg{fill:#fff}
.db-name{
  font-size: 14px;
  font-weight:600;color:var(--text);
  overflow:hidden;text-overflow:ellipsis;white-space:nowrap;
}
.db-sub{font-size: 12px;color:var(--muted);margin-top:3px}
.db-item.selected .db-sub{color:var(--accent2)}

/* ── DB Confirm ── */
.db-confirm-box{
  display:flex;align-items:center;justify-content:space-between;gap:16px;
  background:rgba(15,18,25,0.85);
  border:1.5px solid var(--border);
  border-radius:var(--radius);
  padding:16px 18px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
}
.db-confirm-left{display:flex;align-items:center;gap:14px}
.db-confirm-icon{
  width:42px;height:42px;flex-shrink:0;
  background:rgba(99,102,241,.15);
  border-radius:12px;
  display:flex;align-items:center;justify-content:center;
}
.db-confirm-icon svg{width:20px;height:20px;fill:var(--accent2)}
.db-confirm-name{
  font-size: 17px;
  font-weight:700;color:var(--text);
}
.db-confirm-meta{
  font-size: 13px;
  color:var(--muted);margin-top:4px;
}
.db-confirm-badge{
  font-size: 13px;
  font-weight:700;letter-spacing:.4px;
  padding:6px 14px;border-radius:24px;white-space:nowrap;
  background:rgba(100,116,139,.15);color:var(--muted);
  border:1px solid var(--border);
}
.db-confirm-badge.confirmed{
  background:rgba(16,185,129,.15);color:#34d399;
  border-color:rgba(16,185,129,.35);
}
.db-confirm-warning{
  display:flex;align-items:flex-start;gap:10px;
  background:rgba(245,158,11,.1);
  border:1px solid rgba(245,158,11,.25);
  border-radius:var(--radius);
  padding:12px 15px;
  font-size: 14px;
  color:#fbbf24;margin-top:12px;line-height:1.6;
  box-shadow: 0 2px 8px rgba(245,158,11,0.1);
}
.db-confirm-warning svg{width:16px;height:16px;fill:#fbbf24;flex-shrink:0;margin-top:2px}

/* ── Hint ── */
.hint{
  display:flex;align-items:flex-start;gap:12px;
  background:rgba(99,102,241,.12);
  border:1px solid rgba(99,102,241,.25);
  border-radius:var(--radius);
  padding:14px 16px;
  font-size: 14px;
  color:var(--accent2);
  margin-top:24px;
  line-height:1.6;
  box-shadow: 0 2px 8px rgba(99,102,241,0.1);
}
.hint svg{width:17px;height:17px;fill:var(--accent2);flex-shrink:0;margin-top:2px}
.hint strong{color:#c7d2fe}

/* ── Spinner ── */
.spinner{
  width:16px;height:16px;
  border:2.5px solid rgba(255,255,255,.25);
  border-top-color:#fff;
  border-radius:50%;
  animation:spin .6s linear infinite;
  flex-shrink:0;
}
@keyframes spin{to{transform:rotate(360deg)}}

/* ── Modal ── */
.modal-overlay{
  position:fixed;inset:0;
  background:rgba(0,0,0,.8);
  display:none;align-items:center;justify-content:center;
  z-index:100;padding:24px;
  backdrop-filter: blur(4px);
}
.modal-overlay.active{display:flex}
.modal{
  background:var(--surface);
  border:1px solid var(--border);
  border-radius:18px;
  width:100%;max-width:750px;
  max-height:85vh;
  display:flex;flex-direction:column;
  overflow:hidden;
  box-shadow: 0 25px 60px rgba(0,0,0,0.6);
}
.modal-head{
  padding:20px 24px;
  border-bottom:1px solid var(--border);
  display:flex;justify-content:space-between;align-items:center;
  background: linear-gradient(135deg, rgba(99,102,241,0.06) 0%, transparent 100%);
}
.modal-head h2{font-size: 18px;font-weight:700}
.close-btn{
  background:none;border:none;cursor:pointer;
  color:var(--muted);padding:6px;border-radius:8px;display:flex;
  transition: all 0.2s ease;
}
.close-btn:hover{color:var(--text);background:var(--border)}
.close-btn svg{width:22px;height:22px;fill:currentColor}
.modal-body{padding:20px 24px;overflow-y:auto;flex:1}
.modal-foot{
  padding:16px 24px;
  border-top:1px solid var(--border);
  display:flex;justify-content:flex-end;
  gap: 10px;
}
.log-output{
  background:var(--bg);color:#a3e635;
  padding:18px;border-radius:12px;
  font-family:'Courier New',monospace;
  font-size: 14px;
  line-height:1.7;white-space:pre-wrap;word-break:break-all;
  border: 1px solid var(--border);
}

/* ── Responsive ── */
@media (max-width: 1024px) {
  .card{ max-width: 700px }
  .header{ padding: 28px 32px }
  .body{ padding: 32px }
}

@media (max-width: 768px) {
  .card{ max-width: 90% }
  .header{ 
    padding: 24px;
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  .header-badges{ 
    flex-direction: row;
    align-items: flex-start;
  }
  .body{ padding: 24px }
  .grid-2{ grid-template-columns:1fr }
  .db-grid{ grid-template-columns:1fr 1fr }
}

@media (max-width: 540px) {
  .header-badges{ display:none }
  .grid-2{ grid-template-columns:1fr }
  .db-grid{ grid-template-columns:1fr }
  .logo{ width: 56px; height: 56px }
  .header-text h1{ font-size: 22px }
}
</style>
</head>
<body>

<!-- Canvas untuk background langit malam, nebula, dan bintang -->
<canvas id="starCanvas"></canvas>

<div class="card">

  <!-- Header -->
  <div class="header">
    <div class="logo">
      <img src="Icon.png" alt="Kasir Lancar">
    </div>
    <div class="header-text">
      <h1>Kasir Lancar</h1>
      <p>Konfigurasi server &amp; database untuk aplikasi mobile</p>
      <div id="connectionStatus" style="display:none;margin-top:10px">
        <span class="badge badge-android" id="connectionBadge">
          <svg viewBox="0 0 24 24"><path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/></svg>
          <span id="connectionText">Terhubung</span>
        </span>
      </div>
    </div>
    <div class="header-badges">
      <span class="badge badge-android">
        <svg viewBox="0 0 24 24"><path d="M17.6 9.48l1.84-3.18c.16-.31.04-.69-.26-.85-.29-.15-.65-.06-.83.22l-1.88 3.24A10.7 10.7 0 0012 8c-1.53 0-2.98.33-4.27.91L5.85 5.67c-.19-.28-.54-.37-.83-.22-.3.16-.42.54-.26.85L6.6 9.48A10.26 10.26 0 002 18h20a10.26 10.26 0 00-4.4-8.52zM7 15.25a1.25 1.25 0 110-2.5 1.25 1.25 0 010 2.5zm10 0a1.25 1.25 0 110-2.5 1.25 1.25 0 010 2.5z"/></svg>
        Android App
      </span>
      <span class="badge badge-wifi">
        <svg viewBox="0 0 24 24"><path d="M1 9l2 2c4.97-4.97 13.03-4.97 18 0l2-2C16.93 2.93 7.08 2.93 1 9zm8 8l3 3 3-3a4.237 4.237 0 00-6 0zm-4-4l2 2a7.074 7.074 0 0110 0l2-2C15.14 9.14 8.87 9.14 5 13z"/></svg>
        WiFi LAN
      </span>
    </div>
  </div>

  <!-- Body -->
  <div class="body">

    <div id="status" class="status"></div>

    <form id="configForm">
      <div class="section-label">Koneksi Database</div>

      <div class="grid-2">
        <div class="field">
          <label>Host</label>
          <input type="text" name="host" id="host" value="localhost" placeholder="localhost">
        </div>
        <div class="field">
          <label>Port</label>
          <input type="number" name="port" id="port" value="3306" placeholder="3306">
        </div>
      </div>

      <div style="height:10px"></div>

      <div class="grid-2">
        <div class="field">
          <label>Username</label>
          <input type="text" name="username" id="username" value="root" placeholder="root">
        </div>
        <div class="field">
          <label>Password</label>
          <div class="field-wrap">
            <input type="password" name="password" id="password" placeholder="••••••••" style="padding-right:34px">
            <button type="button" class="eye-btn" id="togglePassword">
              <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/></svg>
            </button>
          </div>
        </div>
      </div>

      <div class="btn-row">
        <button type="button" class="btn btn-primary btn-full" id="testBtn">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor"><path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/></svg>
          Test Koneksi
        </button>
        <button type="button" class="btn btn-ghost" id="debugBtn" style="display:none">
          Debug
        </button>
      </div>

      <!-- Database List -->
      <div id="databaseList" class="db-section">
        <div class="divider"></div>
        <div class="section-label">Pilih Database</div>
        <div id="databaseGrid" class="db-grid"></div>

        <!-- Konfirmasi database yang dipilih -->
        <div id="dbConfirm" style="display:none;margin-top:12px">
          <div class="db-confirm-box">
            <div class="db-confirm-left">
              <div class="db-confirm-icon">
                <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 3C7.58 3 4 4.79 4 7v10c0 2.21 3.58 4 8 4s8-1.79 8-4V7c0-2.21-3.58-4-8-4zm0 2c3.87 0 6 1.5 6 2s-2.13 2-6 2-6-1.5-6-2 2.13-2 6-2z"/></svg>
              </div>
              <div>
                <div class="db-confirm-name" id="dbConfirmName">—</div>
                <div class="db-confirm-meta" id="dbConfirmMeta">—</div>
              </div>
            </div>
            <div class="db-confirm-badge" id="dbConfirmBadge">Belum dikonfirmasi</div>
          </div>
          <div class="db-confirm-warning" id="dbConfirmWarning" style="display:none">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"/></svg>
            Pastikan database ini adalah database AppKasir yang benar. Konfigurasi yang salah dapat menyebabkan data tidak terbaca.
          </div>
          <div style="display:flex;gap:8px;margin-top:10px">
            <button type="button" class="btn btn-ghost" id="btnBatalPilih" style="flex:1">
              <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg>
              Batal
            </button>
            <button type="button" class="btn btn-primary" id="btnKonfirmasiDB" style="flex:2">
              <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/></svg>
              Ya, Gunakan Database Ini
            </button>
          </div>
        </div>
      </div>

      <!-- Save -->
      <div style="margin-top:16px">
        <button type="submit" class="btn btn-success btn-full" id="saveBtn" disabled>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor"><path d="M17 3H5a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2V7l-4-4zm-5 16a3 3 0 110-6 3 3 0 010 6zm3-10H5V5h10v4z"/></svg>
          Simpan Konfigurasi
        </button>
      </div>

    </form>

    <div class="hint">
      <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z"/></svg>
      <span>Halaman ini diakses dari browser PC/server. Setelah konfigurasi disimpan, buka aplikasi <strong>Kasir Lancar</strong> di Android → masukkan IP server ini → login.</span>
    </div>

  </div><!-- /body -->
</div><!-- /card -->

<!-- Debug Modal -->
<div class="modal-overlay" id="debugModal">
  <div class="modal">
    <div class="modal-head">
      <h2>Debug Info</h2>
      <button class="close-btn" id="closeModal">
        <svg viewBox="0 0 24 24"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg>
      </button>
    </div>
    <div class="modal-body">
      <pre class="log-output" id="logOutput"></pre>
    </div>
    <div class="modal-foot">
      <button class="btn btn-primary" id="copyLog">
        <svg viewBox="0 0 24 24" width="14" height="14" fill="currentColor"><path d="M16 1H4a2 2 0 00-2 2v14h2V3h12V1zm3 4H8a2 2 0 00-2 2v14a2 2 0 002 2h11a2 2 0 002-2V7a2 2 0 00-2-2zm0 16H8V7h11v14z"/></svg>
        Copy
      </button>
    </div>
  </div>
</div>

<script>
  // ========== FUNGSI KONFIGURASI ASLI (TIDAK BERUBAH) ==========
  let currentConfig = <?php
    if (file_exists('config.php')) {
      $cfg = include 'config.php';
      unset($cfg['password']);
      echo json_encode($cfg);
    } else {
      echo json_encode(['host'=>'localhost','db_name'=>'db_kasir','username'=>'root','port'=>3306]);
    }
  ?>;

  let selectedDatabase = '';
  let pendingDatabase  = '';
  let debugData = null;

  document.getElementById('host').value     = currentConfig.host     || 'localhost';
  document.getElementById('port').value     = currentConfig.port     || 3306;
  document.getElementById('username').value = currentConfig.username || 'root';

  document.getElementById('togglePassword').addEventListener('click', function() {
    const p = document.getElementById('password');
    p.type = p.type === 'password' ? 'text' : 'password';
  });

  function showStatus(msg, ok) {
    const el = document.getElementById('status');
    el.textContent = msg;
    el.className = 'status ' + (ok ? 'status-success' : 'status-error');
    el.style.display = 'block';
    setTimeout(() => el.style.display = 'none', 5000);
  }

  function renderDatabaseList(databases) {
    const grid = document.getElementById('databaseGrid');
    grid.innerHTML = '';
    hideConfirm();

    if (!databases.length) {
      grid.innerHTML = '<p style="color:var(--muted);font-size:12px;padding:12px 0">Tidak ada database ditemukan</p>';
    } else {
      databases.forEach(db => {
        const item = document.createElement('div');
        item.className = 'db-item';
        const isActive = db.name === currentConfig.db_name;
        if (isActive) {
          item.classList.add('selected');
          selectedDatabase = db.name;
          document.getElementById('saveBtn').disabled = false;
        }
        item.innerHTML = `
          <div class="db-icon">
            <svg viewBox="0 0 24 24"><path d="M12 3C7.58 3 4 4.79 4 7v10c0 2.21 3.58 4 8 4s8-1.79 8-4V7c0-2.21-3.58-4-8-4zm0 2c3.87 0 6 1.5 6 2s-2.13 2-6 2-6-1.5-6-2 2.13-2 6-2z"/></svg>
          </div>
          <div>
            <div class="db-name">${db.name}</div>
            <div class="db-sub">${isActive ? '✓ Aktif' : 'Klik untuk memilih'}</div>
          </div>`;
        item.addEventListener('click', () => showConfirm(db, item));
        grid.appendChild(item);
      });

      if (currentConfig.db_name) {
        showConfirmConfirmed(currentConfig.db_name, currentConfig.host, currentConfig.port);
      }
    }
    document.getElementById('databaseList').style.display = 'block';
  }

  function showConfirm(db, element) {
    document.querySelectorAll('.db-item').forEach(el => {
      el.classList.remove('selected');
      const s = el.querySelector('.db-sub'); if (s) s.textContent = 'Klik untuk memilih';
    });
    element.classList.add('selected');
    const s = element.querySelector('.db-sub'); if (s) s.textContent = '⏳ Menunggu konfirmasi';

    pendingDatabase = db.name;
    selectedDatabase = '';
    document.getElementById('saveBtn').disabled = true;

    document.getElementById('dbConfirmName').textContent = db.name;
    document.getElementById('dbConfirmMeta').textContent =
      `${document.getElementById('host').value}:${document.getElementById('port').value}`;
    document.getElementById('dbConfirmBadge').textContent = 'Belum dikonfirmasi';
    document.getElementById('dbConfirmBadge').className   = 'db-confirm-badge';
    document.getElementById('dbConfirmWarning').style.display = 'flex';
    document.getElementById('dbConfirm').style.display = 'block';
  }

  function showConfirmConfirmed(dbName, host, port) {
    document.getElementById('dbConfirmName').textContent = dbName;
    document.getElementById('dbConfirmMeta').textContent = `${host}:${port}`;
    document.getElementById('dbConfirmBadge').textContent = '✓ Dikonfirmasi';
    document.getElementById('dbConfirmBadge').className   = 'db-confirm-badge confirmed';
    document.getElementById('dbConfirmWarning').style.display = 'none';
    document.getElementById('dbConfirm').style.display = 'block';
  }

  function hideConfirm() {
    document.getElementById('dbConfirm').style.display = 'none';
    pendingDatabase  = '';
  }

  document.getElementById('btnKonfirmasiDB').addEventListener('click', function() {
    if (!pendingDatabase) return;
    selectedDatabase = pendingDatabase;
    pendingDatabase  = '';
    document.getElementById('saveBtn').disabled = false;

    document.querySelectorAll('.db-item').forEach(el => {
      const nameEl = el.querySelector('.db-name');
      const subEl  = el.querySelector('.db-sub');
      if (nameEl && nameEl.textContent === selectedDatabase) {
        if (subEl) subEl.textContent = '✓ Dipilih';
      }
    });

    showConfirmConfirmed(
      selectedDatabase,
      document.getElementById('host').value,
      document.getElementById('port').value
    );
    showStatus(`Database "${selectedDatabase}" siap digunakan`, true);
  });

  document.getElementById('btnBatalPilih').addEventListener('click', function() {
    document.querySelectorAll('.db-item').forEach(el => {
      const nameEl = el.querySelector('.db-name');
      const subEl  = el.querySelector('.db-sub');
      if (!nameEl || !subEl) return;
      if (selectedDatabase && nameEl.textContent === selectedDatabase) {
        el.classList.add('selected');
        subEl.textContent = '✓ Dipilih';
      } else {
        el.classList.remove('selected');
        subEl.textContent = 'Klik untuk memilih';
      }
    });
    pendingDatabase = '';
    if (selectedDatabase) {
      showConfirmConfirmed(
        selectedDatabase,
        document.getElementById('host').value,
        document.getElementById('port').value
      );
    } else {
      hideConfirm();
    }
  });

  document.getElementById('testBtn').addEventListener('click', async function() {
    const btn = this;
    const orig = btn.innerHTML;
    btn.innerHTML = '<span class="spinner"></span><span>Menguji...</span>';
    btn.disabled = true;
    document.getElementById('databaseList').style.display = 'none';
    document.getElementById('debugBtn').style.display = 'none';
    selectedDatabase = ''; debugData = null;
    document.getElementById('saveBtn').disabled = true;

    const fd = new FormData(document.getElementById('configForm'));
    const cfg = { host: fd.get('host'), port: parseInt(fd.get('port')), username: fd.get('username'), password: fd.get('password') };

    try {
      const res  = await fetch('test_db_connection.php', { method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify(cfg) });
      const data = await res.json();
      if (data.status === 'success') {
        showStatus('Koneksi berhasil — pilih database', true);
        renderDatabaseList(data.databases);
      } else {
        showStatus('Gagal: ' + data.message, false);
        debugData = data;
        document.getElementById('debugBtn').style.display = 'inline-flex';
      }
    } catch(e) {
      showStatus('Error: ' + e.message, false);
      debugData = { error: e.message };
      document.getElementById('debugBtn').style.display = 'inline-flex';
    } finally {
      btn.innerHTML = orig; btn.disabled = false;
    }
  });

  document.getElementById('configForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    if (!selectedDatabase) { showStatus('Pilih database terlebih dahulu', false); return; }

    const btn = document.getElementById('saveBtn');
    const orig = btn.innerHTML;
    btn.innerHTML = '<span class="spinner"></span><span>Menyimpan...</span>';
    btn.disabled = true;

    const fd  = new FormData(this);
    const cfg = { host: fd.get('host'), port: parseInt(fd.get('port')), db_name: selectedDatabase, username: fd.get('username'), password: fd.get('password'), charset: 'utf8mb4' };

    try {
      const res  = await fetch('save_config.php', { method:'POST', headers:{'Content-Type':'application/json','X-Admin-Token':'kasir-admin-2026'}, body: JSON.stringify(cfg) });
      const data = await res.json();
      showStatus(data.status === 'success' ? 'Konfigurasi berhasil disimpan' : 'Gagal: ' + data.message, data.status === 'success');
      if (data.status === 'success') currentConfig = cfg;
    } catch(e) {
      showStatus('Error: ' + e.message, false);
    } finally {
      btn.innerHTML = orig; btn.disabled = false;
    }
  });

  document.getElementById('debugBtn').addEventListener('click', function() {
    if (!debugData) return;
    document.getElementById('logOutput').textContent = JSON.stringify(debugData, null, 2);
    document.getElementById('debugModal').classList.add('active');
  });

  document.getElementById('closeModal').addEventListener('click', () => document.getElementById('debugModal').classList.remove('active'));
  document.getElementById('debugModal').addEventListener('click', function(e) { if (e.target === this) this.classList.remove('active'); });

  document.getElementById('copyLog').addEventListener('click', async function() {
    try {
      await navigator.clipboard.writeText(document.getElementById('logOutput').textContent);
      const orig = this.innerHTML;
      this.innerHTML = '<svg viewBox="0 0 24 24" width="14" height="14" fill="currentColor"><path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/></svg> Copied!';
      setTimeout(() => this.innerHTML = orig, 2000);
    } catch { showStatus('Gagal copy', false); }
  });

  // ========== AUTO-LOAD CONNECTION ON PAGE STARTUP ==========
  async function autoLoadConnection() {
    // Check if config.php exists and has database configured
    let hasConfig, configDbName, configHost, configPort, configUsername;
    
    <?php if (file_exists('config.php')): ?>
    hasConfig = true;
    configDbName = currentConfig.db_name || '';
    configHost = currentConfig.host || 'localhost';
    configPort = currentConfig.port || 3306;
    configUsername = currentConfig.username || 'root';
    <?php else: ?>
    hasConfig = false;
    configDbName = '';
    configHost = 'localhost';
    configPort = 3306;
    configUsername = 'root';
    <?php endif; ?>

    if (!hasConfig || !configDbName) {
      return; // No config yet, user needs to configure manually
    }

    // Show loading status
    const statusEl = document.getElementById('connectionStatus');
    const badgeEl = document.getElementById('connectionBadge');
    const textEl = document.getElementById('connectionText');
    
    statusEl.style.display = 'block';
    textEl.textContent = 'Memeriksa koneksi...';
    badgeEl.style.background = 'rgba(100,116,139,.15)';
    badgeEl.style.color = 'var(--muted)';
    badgeEl.style.borderColor = 'var(--border)';

    try {
      // Test connection with saved credentials (password injected server-side)
      const cfg = { 
        host: configHost, 
        port: parseInt(configPort), 
        username: configUsername, 
        password: <?php echo file_exists('config.php') ? json_encode((include 'config.php')['password'] ?? '') : "''"; ?>
      };

      const res = await fetch('test_db_connection.php', { 
        method: 'POST', 
        headers: {'Content-Type': 'application/json'}, 
        body: JSON.stringify(cfg) 
      });
      const data = await res.json();

      if (data.status === 'success') {
        // Connection successful
        selectedDatabase = configDbName;
        document.getElementById('saveBtn').disabled = false;
        
        // Show connected status
        textEl.textContent = `Terhubung ke ${configDbName}`;
        badgeEl.style.background = 'rgba(16,185,129,.15)';
        badgeEl.style.color = '#34d399';
        badgeEl.style.borderColor = 'rgba(16,185,129,.3)';

        // Render database list with current database highlighted
        renderDatabaseList(data.databases);
        
        // Show success message
        showStatus(`✓ Database "${configDbName}" sudah terhubung dan aktif`, true);
      } else {
        // Connection failed
        textEl.textContent = 'Koneksi gagal';
        badgeEl.style.background = 'rgba(239,68,68,.15)';
        badgeEl.style.color = '#f87171';
        badgeEl.style.borderColor = 'rgba(239,68,68,.3)';
        
        showStatus('Gagal terhubung: ' + data.message, false);
        debugData = data;
        document.getElementById('debugBtn').style.display = 'inline-flex';
      }
    } catch(e) {
      textEl.textContent = 'Error koneksi';
      badgeEl.style.background = 'rgba(239,68,68,.15)';
      badgeEl.style.color = '#f87171';
      badgeEl.style.borderColor = 'rgba(239,68,68,.3)';
      
      showStatus('Error: ' + e.message, false);
      debugData = { error: e.message };
      document.getElementById('debugBtn').style.display = 'inline-flex';
    }
  }

  // Run auto-load when page is ready
  document.addEventListener('DOMContentLoaded', function() {
    autoLoadConnection();
  });
</script>

<!-- ========== BACKGROUND ANIMASI LANGIT MALAM + NEBULA + RASI BINTANG ========== -->
<script>
  (function() {
    const canvas = document.getElementById('starCanvas');
    const ctx = canvas.getContext('2d');
    let width, height;
    let animationId;
    let time = 0; // untuk efek gerakan lambat

    // ---- Bintang ----
    const STAR_COUNT = 800;
    let stars = [];
    // ---- Nebula (awan berwarna) ----
    let nebulae = [];
    // ---- Rasi bintang (constellations) ----
    let constellations = [];

    function init() {
      resize();
      window.addEventListener('resize', resize);
      generateStars();
      generateNebulae();
      generateConstellations();
      animate();
    }

    function resize() {
      width = window.innerWidth;
      height = window.innerHeight;
      canvas.width = width;
      canvas.height = height;
    }

    function generateStars() {
      stars = [];
      for (let i = 0; i < STAR_COUNT; i++) {
        stars.push({
          x: Math.random() * width,
          y: Math.random() * height,
          radius: Math.random() * 2.2 + 0.8,
          alpha: Math.random() * 0.6 + 0.2,
          speed: 0.002 + Math.random() * 0.008,
          phase: Math.random() * Math.PI * 2,
          twinkleSpeed: 0.01 + Math.random() * 0.03
        });
      }
    }

    function generateNebulae() {
      // Nebula berupa lingkaran besar dengan gradien radial, bergerak sangat lambat
      nebulae = [
        { x: 0.2, y: 0.3, rad: 0.4, r: 80, g: 70, b: 200, a: 0.12, vx: 0.0001, vy: 0.00005 },
        { x: 0.7, y: 0.6, rad: 0.5, r: 180, g: 80, b: 150, a: 0.10, vx: -0.00008, vy: 0.00012 },
        { x: 0.5, y: 0.8, rad: 0.45, r: 40, g: 100, b: 210, a: 0.09, vx: 0.00007, vy: -0.0001 },
        { x: 0.1, y: 0.7, rad: 0.35, r: 210, g: 60, b: 100, a: 0.08, vx: 0.00005, vy: 0.00009 },
        { x: 0.85, y: 0.2, rad: 0.38, r: 100, g: 40, b: 180, a: 0.11, vx: -0.00012, vy: -0.00006 }
      ];
      // Simpan posisi asli untuk gerakan relatif
      nebulae.forEach(n => {
        n.originX = n.x;
        n.originY = n.y;
      });
    }

    function generateConstellations() {
      // Membuat beberapa kelompok bintang yang dihubungkan garis (rasi bintang)
      // Setiap rasi punya titik-titik relatif terhadap lebar/tinggi
      const cons = [
        { // Orion-like
          points: [[0.2,0.3],[0.25,0.35],[0.22,0.4],[0.28,0.45],[0.32,0.42],[0.3,0.38]],
          color: 'rgba(210, 180, 140, 0.6)',
          lineWidth: 1.2
        },
        { // Big Dipper
          points: [[0.7,0.2],[0.73,0.18],[0.78,0.19],[0.8,0.23],[0.78,0.28],[0.74,0.27],[0.71,0.24]],
          color: 'rgba(200, 200, 220, 0.55)',
          lineWidth: 1
        },
        { // Cassiopeia (W shape)
          points: [[0.45,0.15],[0.48,0.12],[0.5,0.16],[0.52,0.12],[0.55,0.15]],
          color: 'rgba(190, 190, 210, 0.5)',
          lineWidth: 1
        },
        { // Leo
          points: [[0.35,0.7],[0.38,0.68],[0.42,0.69],[0.44,0.72],[0.41,0.75],[0.37,0.74]],
          color: 'rgba(220, 200, 160, 0.55)',
          lineWidth: 1.1
        }
      ];
      constellations = cons;
    }

    // Update posisi nebula berdasarkan waktu (gerakan melingkar sangat lambat)
    function updateNebulae(t) {
      nebulae.forEach(n => {
        // Pergerakan sinus/cosinus lembut
        n.x = n.originX + Math.sin(t * n.vx * 50) * 0.03;
        n.y = n.originY + Math.cos(t * n.vy * 50) * 0.03;
      });
    }

    function drawStars() {
      for (let s of stars) {
        // Efek berkedip (twinkle)
        const twinkle = Math.sin(time * s.twinkleSpeed + s.phase) * 0.3 + 0.7;
        const alpha = Math.min(0.9, s.alpha * twinkle);
        ctx.beginPath();
        ctx.arc(s.x, s.y, s.radius, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(255, 240, 210, ${alpha})`;
        ctx.fill();
      }
    }

    function drawNebulae() {
      for (let n of nebulae) {
        const cx = n.x * width;
        const cy = n.y * height;
        const rad = n.rad * Math.min(width, height);
        const gradient = ctx.createRadialGradient(cx, cy, rad * 0.2, cx, cy, rad);
        gradient.addColorStop(0, `rgba(${n.r}, ${n.g}, ${n.b}, ${n.a * 0.8})`);
        gradient.addColorStop(0.5, `rgba(${n.r}, ${n.g}, ${n.b}, ${n.a * 0.4})`);
        gradient.addColorStop(1, `rgba(${n.r}, ${n.g}, ${n.b}, 0)`);
        ctx.globalCompositeOperation = 'lighter';
        ctx.beginPath();
        ctx.arc(cx, cy, rad, 0, Math.PI * 2);
        ctx.fillStyle = gradient;
        ctx.fill();
      }
      ctx.globalCompositeOperation = 'source-over';
    }

    function drawConstellations() {
      for (let cons of constellations) {
        ctx.beginPath();
        ctx.strokeStyle = cons.color;
        ctx.lineWidth = cons.lineWidth;
        // Gambar garis antar titik
        const points = cons.points.map(p => ({ x: p[0] * width, y: p[1] * height }));
        for (let i = 0; i < points.length - 1; i++) {
          ctx.beginPath();
          ctx.moveTo(points[i].x, points[i].y);
          ctx.lineTo(points[i+1].x, points[i+1].y);
          ctx.stroke();
        }
        // Gambar bintang di setiap titik rasi (lebih terang)
        for (let p of points) {
          ctx.beginPath();
          ctx.arc(p.x, p.y, 2.5, 0, Math.PI * 2);
          ctx.fillStyle = 'rgba(255, 255, 210, 0.9)';
          ctx.fill();
          ctx.beginPath();
          ctx.arc(p.x, p.y, 1.2, 0, Math.PI * 2);
          ctx.fillStyle = 'white';
          ctx.fill();
        }
      }
    }

    // Efek bintang jatuh (shooting star) - sederhana
    let shootingStars = [];
    function addShootingStar() {
      if (Math.random() < 0.005) { // probabilitas per frame
        shootingStars.push({
          x: Math.random() * width,
          y: Math.random() * height * 0.5,
          vx: (Math.random() - 0.5) * 3 + 2,
          vy: (Math.random() - 0.5) * 1.5 + 1.5,
          life: 1.0,
          length: 15
        });
      }
    }

    function updateShootingStars() {
      for (let i = 0; i < shootingStars.length; i++) {
        let s = shootingStars[i];
        s.x += s.vx;
        s.y += s.vy;
        s.life -= 0.02;
        if (s.x > width + 50 || s.x < -50 || s.y > height + 50 || s.y < -50 || s.life <= 0) {
          shootingStars.splice(i,1);
          i--;
        }
      }
    }

    function drawShootingStars() {
      for (let s of shootingStars) {
        ctx.beginPath();
        ctx.moveTo(s.x, s.y);
        ctx.lineTo(s.x - s.vx * s.length, s.y - s.vy * s.length);
        ctx.strokeStyle = `rgba(255, 255, 200, ${s.life * 0.8})`;
        ctx.lineWidth = 2;
        ctx.stroke();
        // kepala bintang
        ctx.beginPath();
        ctx.arc(s.x, s.y, 1.5, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(255, 245, 190, ${s.life})`;
        ctx.fill();
      }
    }

    // Animasi utama
    function animate() {
      if (!ctx) return;
      time += 0.016; // asumsi 60fps, gerakan halus
      updateNebulae(time);
      addShootingStar();
      updateShootingStars();

      // Bersihkan canvas dengan gradien gelap langit malam
      const grad = ctx.createLinearGradient(0, 0, 0, height);
      grad.addColorStop(0, '#03050b');
      grad.addColorStop(0.6, '#0a0c18');
      grad.addColorStop(1, '#10121f');
      ctx.fillStyle = grad;
      ctx.fillRect(0, 0, width, height);

      // Gambar nebula (di belakang bintang)
      drawNebulae();
      // Gambar bintang
      drawStars();
      // Gambar rasi bintang
      drawConstellations();
      // Gambar bintang jatuh
      drawShootingStars();

      animationId = requestAnimationFrame(animate);
    }

    init();
  })();
</script>
</body>
</html>