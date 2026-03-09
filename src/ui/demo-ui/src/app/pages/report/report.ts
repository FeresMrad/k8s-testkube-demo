import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './report.html',
  styleUrl: './report.scss'
})
export class ReportComponent {
  constructor(
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {}

  downloadReport(): void {
    this.productService.downloadReport().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `product-report-${new Date().toISOString().slice(0, 10)}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);
        this.snackBar.open('Report downloaded successfully!', 'Close', { duration: 3000 });
      },
      error: () => {
        this.snackBar.open('Failed to download report. Please try again.', 'Close', { duration: 4000 });
      }
    });
  }
}