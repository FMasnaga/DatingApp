import { Component, OnInit } from '@angular/core';
import { User } from 'src/models/User';
import { AlertifyService } from '../../_services/Alertify.service';
import { UserService } from '../../_services/User.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/models/Pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users : User[];
  pagination : Pagination;
  user : User = JSON.parse(localStorage.getItem('user'));
  userParams : any ={};
  genderList = [{value:'male', display : 'Males'},{value:'female', display:'Females'}];

  constructor(private userService: UserService, private alertify: AlertifyService,
    private route :ActivatedRoute) {

   }

  ngOnInit() {
    this.route.data.subscribe(data =>{
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.userParams.gender = this.user.gender === "female" ? "male" : "female";
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99; 
    this.userParams.orderBy = "lastActive";
  }

  resetFilter (){
    this.userParams.gender = this.user.gender === "female" ? "male" : "female";
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99; 
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers(){
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemPerPage, this.userParams).subscribe((res: PaginatedResult<User[]>)=>{
      this.users = res.result;
      this.pagination = res.pagination;
    }, error =>{
      this.alertify.error(error);
    })
  }


}
