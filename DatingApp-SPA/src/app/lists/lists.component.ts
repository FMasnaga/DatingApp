import { Component, OnInit } from '@angular/core';
import { User } from 'src/models/User';
import { Pagination, PaginatedResult } from 'src/models/Pagination';
import { AuthService } from '../_services/Auth.service';
import { UserService } from '../_services/User.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/Alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users : User[];
  pagination: Pagination ;
  likeParam: string;

  constructor(private authService: AuthService, private userService : UserService, private route : ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data =>{
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.likeParam = 'UserLike';
  }
  
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
  
  loadUsers(){
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemPerPage, null, this.likeParam).subscribe((res: PaginatedResult<User[]>)=>{
      this.users = res.result;
      this.pagination = res.pagination;
    }, error =>{
      this.alertify.error(error);
    })
  }

}
