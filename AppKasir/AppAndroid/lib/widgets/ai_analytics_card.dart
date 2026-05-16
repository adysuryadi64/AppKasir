import 'package:flutter/material.dart';

/// Kartu ringkasan AI Analytics — dipakai 6x di dashboard.
/// Klik → buka modal detail via [onTap].
class AIAnalyticsCard extends StatelessWidget {
  final String title;
  final IconData icon;
  final Color color;
  final String? keyMetric; // angka/teks utama, null = belum ada data
  final String? insight; // 1 kalimat insight
  final bool isLoading;
  final VoidCallback onTap;

  const AIAnalyticsCard({
    super.key,
    required this.title,
    required this.icon,
    required this.color,
    required this.onTap,
    this.keyMetric,
    this.insight,
    this.isLoading = false,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: isLoading ? null : onTap,
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: color.withValues(alpha: 0.2)),
          boxShadow: [
            BoxShadow(
              color: color.withValues(alpha: 0.08),
              blurRadius: 8,
              offset: const Offset(0, 3),
            ),
          ],
        ),
        child: isLoading ? _buildSkeleton() : _buildContent(),
      ),
    );
  }

  Widget _buildContent() {
    final noData = keyMetric == null;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Icon + arrow
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Icon(icon, color: color, size: 20),
            ),
            Icon(
              Icons.arrow_forward_ios,
              size: 12,
              color: noData
                  ? Colors.grey.shade300
                  : color.withValues(alpha: 0.5),
            ),
          ],
        ),
        const SizedBox(height: 10),
        // Judul
        Text(
          title,
          style: TextStyle(
            fontSize: 11,
            fontWeight: FontWeight.w600,
            color: Colors.grey.shade600,
          ),
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
        ),
        const SizedBox(height: 4),
        // Key metric
        if (noData)
          Text(
            'Data belum cukup',
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey.shade400,
              fontStyle: FontStyle.italic,
            ),
          )
        else
          Text(
            keyMetric!,
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.bold,
              color: color,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        // Insight
        if (insight != null && !noData) ...[
          const SizedBox(height: 3),
          Text(
            insight!,
            style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ],
      ],
    );
  }

  Widget _buildSkeleton() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            _shimmerBox(36, 36, radius: 10),
            _shimmerBox(12, 12, radius: 6),
          ],
        ),
        const SizedBox(height: 10),
        _shimmerBox(12, 60),
        const SizedBox(height: 6),
        _shimmerBox(16, 80),
        const SizedBox(height: 4),
        _shimmerBox(10, 100),
      ],
    );
  }

  Widget _shimmerBox(double height, double width, {double radius = 4}) {
    return Container(
      height: height,
      width: width,
      decoration: BoxDecoration(
        color: Colors.grey.shade200,
        borderRadius: BorderRadius.circular(radius),
      ),
    );
  }
}
