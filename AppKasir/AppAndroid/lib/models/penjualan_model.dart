import 'package:json_annotation/json_annotation.dart';

part 'penjualan_model.g.dart';

@JsonSerializable()
class PenjualanModel {
  final String idPenjualan;
  final String idPelanggan;
  final String namaPelanggan;
  final String alamatPelanggan;
  final String jenisPelanggan;
  final String lokasiBarang;
  final DateTime tglTransaksi;
  final double grandTotalSblPajak;
  final double diskonTotalPersen;
  final double diskonTotalRp;
  final double pajakPersen;
  final double pajakRp;
  final double grandTotalStlPajak;
  final double laba;
  final double bayar;
  final double nominalTransfer;
  final double totalHpp;
  final double biayaKirim;
  final double kembali;
  final double sisaTagihan;
  final DateTime jatuhTempo;
  final String statusBayar;
  final String statusTransaksi;
  final String idUser;
  final String idKomputer;
  final List<PenjualanDetailModel> items;

  PenjualanModel({
    required this.idPenjualan,
    required this.idPelanggan,
    required this.namaPelanggan,
    required this.alamatPelanggan,
    required this.jenisPelanggan,
    required this.lokasiBarang,
    required this.tglTransaksi,
    required this.grandTotalSblPajak,
    required this.diskonTotalPersen,
    required this.diskonTotalRp,
    required this.pajakPersen,
    required this.pajakRp,
    required this.grandTotalStlPajak,
    required this.laba,
    required this.bayar,
    required this.nominalTransfer,
    required this.totalHpp,
    required this.biayaKirim,
    required this.kembali,
    required this.sisaTagihan,
    required this.jatuhTempo,
    required this.statusBayar,
    required this.statusTransaksi,
    required this.idUser,
    required this.idKomputer,
    required this.items,
  });

  factory PenjualanModel.fromJson(Map<String, dynamic> json) =>
      _$PenjualanModelFromJson(json);

  Map<String, dynamic> toJson() => _$PenjualanModelToJson(this);
}

@JsonSerializable()
class PenjualanDetailModel {
  final String fakturJual;
  final String idPelanggan;
  final String namaPelanggan;
  final String jenisPelanggan;
  final String lokasiBarang;
  final DateTime tanggalJual;
  final String idBarang;
  final String namaBarang;
  final String serialNumber;
  final double hargaBeli;
  final double qty;
  final String satuan;
  final double isiSatuan;
  final double hargaBeliSatuan;
  final double hargaJual;
  final double qtySatuan;
  final double diskonPersen;
  final double diskonRp;
  final double totalDiskon;
  final double totalHarga;
  final double laba;
  final String idUser;
  final String idKomputer;

  PenjualanDetailModel({
    required this.fakturJual,
    required this.idPelanggan,
    required this.namaPelanggan,
    required this.jenisPelanggan,
    required this.lokasiBarang,
    required this.tanggalJual,
    required this.idBarang,
    required this.namaBarang,
    required this.serialNumber,
    required this.hargaBeli,
    required this.qty,
    required this.satuan,
    required this.isiSatuan,
    required this.hargaBeliSatuan,
    required this.hargaJual,
    required this.qtySatuan,
    required this.diskonPersen,
    required this.diskonRp,
    required this.totalDiskon,
    required this.totalHarga,
    required this.laba,
    required this.idUser,
    required this.idKomputer,
  });

  factory PenjualanDetailModel.fromJson(Map<String, dynamic> json) =>
      _$PenjualanDetailModelFromJson(json);

  Map<String, dynamic> toJson() => _$PenjualanDetailModelToJson(this);
}
