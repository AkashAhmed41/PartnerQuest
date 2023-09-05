import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { ErrorTestComponent } from './_errors/error-test/error-test.component';
import { NotFoundComponent } from './_errors/not-found/not-found.component';
import { ServerErrorComponent } from './_errors/server-error/server-error.component';
import { EditMemberComponent } from './members/edit-member/edit-member.component';
import { NavigationPreventingGuard } from './_guards/navigation-preventing.guard';
import { MemberDetailsResolver } from './_resolvers/member-details.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guards/admin.guard';

const routes: Routes = [
  { path: '', component: HomeComponent},
  { path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent,},
      { path: 'members/:username', component: MemberDetailComponent, resolve: {member: MemberDetailsResolver}},
      { path: ':username/edit', component: EditMemberComponent, canDeactivate: [NavigationPreventingGuard]},
      { path: 'lists', component: ListsComponent},
      { path: 'messages', component: MessagesComponent},
      { path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]}
    ]
  },
  { path: 'errors', component: ErrorTestComponent},
  { path: 'not-found', component: NotFoundComponent},
  { path: 'server-error', component: ServerErrorComponent},
  { path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
