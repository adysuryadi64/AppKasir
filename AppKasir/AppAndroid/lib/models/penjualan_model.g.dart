// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'penjualan_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PenjualanModel _$PenjualanModelFromJson(Map<String, dynamic> json) =>
    PenjualanModel(
      idPenjualan: json['idPenjualan'] as String,
      idPelanggan: json['idPelanggan'] as String,
      namaPelanggan: json['namaPelanggan'] as String,
      alamatPelanggan: json['alamatPelanggan'] as String,
      jenisPelanggan: json['jenisPelanggan'] as String,
      lokasiBarang: json['lokasiBarang'] as String,
      tglTransaksi: DateTime.parse(json['tglTransaksi'] as String),
      grandTotalSblPajak: (json['grandTotalSblPajak'] as num).toDouble(),
      diskonTotalPersen: (json['diskonTotalPersen'] as num).toDouble(),
      diskonTotalRp: (json['diskonTotalRp'] as num).toDouble(),
      pajakPersen: (json['pajakPersen'] as num).toDouble(),
      pajakRp: (json['pajakRp'] as num).toDouble(),
      grandTotalStlPajak: (json['grandTotalStlPajak'] as num).toDouble(),
      laba: (json['laba'] as num).toDouble(),
      bayar: (json['bayar'] as num).toDouble(),
      nominalTransfer: (json['nominalTransfer'] as num).toDouble(),
      totalHpp: (json['totalHpp'] as num).toDouble(),
      biayaKirim: (json['biayaKirim'] as num).toDouble(),
      kembali: (json['kembali'] as num).toDouble(),
      sisaTagihan: (json['sisaTagihan'] as num).toDouble(),
      jatuhTempo: DateTime.parse(json['jatuhTempo'] as String),
      statusBayar: json['statusBayar'] as String,
      statusTransaksi: json['statusTransaksi'] as String,
      idUser: json['idUser'] as String,
      idKomputer: json['idKomputer'] as String,
      items: (json['items'] as List<dynamic>)
          .map((e) => PenjualanDetailModel.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$PenjualanModelToJson(PenjualanModel instance) =>
    <String, dynamic>{
      'idPenjualan': instance.idPenjualan,
      'idPelanggan': instance.idPelanggan,
      'namaPelanggan': instance.namaPelanggan,
      'alamatPelanggan': instance.alamatPelanggan,
      'jenisPelanggan': instance.jenisPelanggan,
      'lokasiBarang': instance.lokasiBarang,
      'tglTransaksi': instance.tglTransaksi.toIso8601String(),
      'grandTotalSblPajak': instance.grandTotalSblPajak,
      'diskonTotalPersen': instance.diskonTotalPersen,
      'diskonTotalRp': instance.diskonTotalRp,
      'pajakPersen': instance.pajakPersen,
      'pajakRp': instance.pajakRp,
      'grandTotalStlPajak': instance.grandTotalStlPajak,
      'laba': instance.laba,
      'bayar': instance.bayar,
      'nominalTransfer': instance.nominalTransfer,
      'totalHpp': instance.totalHpp,
      'biayaKirim': instance.biayaKirim,
      'kembali': instance.kembali,
      'sisaTagihan': instance.sisaTagihan,
      'jatuhTempo': instance.jatuhTempo.toIso8601String(),
      'statusBayar': instance.statusBayar,
      'statusTransaksi': instance.statusTransaksi,
      'idUser': instance.idUser,
      'idKomputer': instance.idKomputer,
      'items': instance.items,
    };

PenjualanDetailModel _$PenjualanDetailModelFromJson(
  Map<String, dynamic> json,
) => PenjualanDetailModel(
  fakturJual: json['fakturJual'] as String,
  idPelanggan: json['idPelanggan'] as String,
  namaPelanggan: json['namaPelanggan'] as String,
  jenisPelanggan: json['jenisPelanggan'] as String,
  lokasiBarang: json['lokasiBarang'] as String,
  tanggalJual: DateTime.parse(json['tanggalJual'] as String),
  idBarang: json['idBarang'] as String,
  namaBarang: json['namaBarang'] as String,
  serialNumber: json['serialNumber'] as String,
  hargaBeli: (json['hargaBeli'] as num).toDouble(),
  qty: (json['qty'] as num).toDouble(),
  satuan: json['satuan'] as String,
  isiSatuan: (json['isiSatuan'] as num).toDouble(),
  hargaBeliSatuan: (json['hargaBeliSatuan'] as num).toDouble(),
  hargaJual: (json['hargaJual'] as num).toDouble(),
  qtySatuan: (json['qtySatuan'] as num).toDouble(),
  diskonPersen: (json['diskonPersen'] as num).toDouble(),
  diskonRp: (json['diskonRp'] as num).toDouble(),
  totalDiskon: (json['totalDiskon'] as num).toDouble(),
  totalHarga: (json['totalHarga'] as num).toDouble(),
  laba: (json['laba'] as num).toDouble(),
  idUser: json['idUser'] as String,
  idKomputer: json['idKomputer'] as String,
);

Map<String, dynamic> _$PenjualanDetailModelToJson(
  PenjualanDetailModel instance,
) => <String, dynamic>{
  'fakturJual': instance.fakturJual,
  'idPelanggan': instance.idPelanggan,
  'namaPelanggan': instance.namaPelanggan,
  'jenisPelanggan': instance.jenisPelanggan,
  'lokasiBarang': instance.lokasiBarang,
  'tanggalJual': instance.tanggalJual.toIso8601String(),
  'idBarang': instance.idBarang,
  'namaBarang': instance.namaBarang,
  'serialNumber': instance.serialNumber,
  'hargaBeli': instance.hargaBeli,
  'qty': instance.qty,
  'satuan': instance.satuan,
  'isiSatuan': instance.isiSatuan,
  'hargaBeliSatuan': instance.hargaBeliSatuan,
  'hargaJual': instance.hargaJual,
  'qtySatuan': instance.qtySatuan,
  'diskonPersen': instance.diskonPersen,
  'diskonRp': instance.diskonRp,
  'totalDiskon': instance.totalDiskon,
  'totalHarga': instance.totalHarga,
  'laba': instance.laba,
  'idUser': instance.idUser,
  'idKomputer': instance.idKomputer,
};
