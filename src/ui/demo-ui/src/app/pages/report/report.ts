import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './report.html',
  styleUrl: './report.scss'
})
export class ReportComponent {
  downloading = false;

  constructor(
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {}

  downloadReport(): void {
    this.downloading = true;

    this.productService.downloadReport().subscribe({
      next: (blob) => {
        // Create a temporary anchor element to trigger the download
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `product-report-${new Date().toISOString().slice(0, 10)}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);

        this.downloading = false;
        this.snackBar.open('Report downloaded successfully!', 'Close', {
          duration: 3000
        });
      },
      error: () => {
        this.downloading = false;
        this.snackBar.open('Failed to download report. Please try again.', 'Close', {
          duration: 4000
        });
      }
    });
  }
}