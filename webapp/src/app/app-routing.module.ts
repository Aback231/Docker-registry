import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { SystemContainers } from './docker-container-system/system-containers-list/system-containers-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { SystemContainersDetailComponent } from './docker-container-system/system-containers-detail/system-containers-detail.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
      { path: 'members/:id', component: MemberDetailComponent },
      { path: 'members/:username', component: MemberDetailComponent },
      //{ path: 'docker/user-containers/:id', component: ListsComponent },
      { path: 'system-containers', component: SystemContainers },
      { path: 'system-containers/:id', component: SystemContainersDetailComponent },
      { path: 'not-found', component: NotFoundComponent},
      { path: 'server-error', component: ServerErrorComponent},
      {path: '**', component: NotFoundComponent, pathMatch: 'full'},
    ]
  },
  { path: '**', component: HomeComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
