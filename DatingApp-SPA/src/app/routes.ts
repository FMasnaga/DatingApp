import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guard/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { PreventUnsavedChanges } from './_guard/preventUnsavedChanges.guard';
import { ListResolver } from './_resolvers/list.resolver';


export const appRoutes: Routes =[
    {path: 'home', component: HomeComponent},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard], resolve:{users:MemberListResolver}},
    {path: 'members/:id', component: MemberDetailComponent, canActivate: [AuthGuard], resolve:{user:MemberDetailResolver}},
    {path: 'member/edit', component: MemberEditComponent, canActivate: [AuthGuard], resolve:{user: MemberEditResolver},
        canDeactivate:[PreventUnsavedChanges]},
    {path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
    {path: 'lists', component: ListsComponent, canActivate: [AuthGuard], resolve: {users : ListResolver}},
    {path: '**', redirectTo: 'home', pathMatch:'full'}
]