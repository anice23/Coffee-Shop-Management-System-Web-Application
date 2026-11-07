import { Routes } from '@angular/router';

import { Login } from './login/login';
import { MainScreen } from './main-screen/main-screen';
import { Products } from './products/products';
import { Categories } from './categories/categories';
import { Orders } from './orders/orders';
import { Users } from './users/users';
import { Reports } from './reports/reports';
import { Settings } from './settings/settings';
import { Pos } from './pos/pos';

import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [

  {
    path: 'login',
    component: Login
  },

  {
    path: 'main',
    component: MainScreen,
    canActivate: [authGuard]
  },

  {
    path: 'products',
    component: Products,
    canActivate: [authGuard]
  },

  {
    path: 'categories',
    component: Categories,
    canActivate: [authGuard]
  },

  {
    path: 'orders',
    component: Orders,
    canActivate: [authGuard]
  },

  {
    path: 'users',
    component: Users,
    canActivate: [authGuard, adminGuard]
  },

  {
    path: 'reports',
    component: Reports,
    canActivate: [authGuard]
  },

  {
    path: 'settings',
    component: Settings,
    canActivate: [authGuard, adminGuard]
  },

  {
    path: 'pos',
    component: Pos,
    canActivate: [authGuard]
  },

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: '**',
    redirectTo: 'login'
  }
];