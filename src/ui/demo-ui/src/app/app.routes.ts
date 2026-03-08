import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/home/home').then(m => m.HomeComponent)
  },
  {
    path: 'products',
    loadComponent: () =>
      import('./pages/products/products').then(m => m.ProductsComponent)
  },
  {
    path: 'products/:id',
    loadComponent: () =>
      import('./pages/product-details/product-details').then(m => m.ProductDetailsComponent)
  },
  {
    path: 'report',
    loadComponent: () =>
      import('./pages/report/report').then(m => m.ReportComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];