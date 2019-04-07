import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/models/User';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/Alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/User.service';
import { AuthService } from 'src/app/_services/Auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm : NgForm;
  user : User;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty){
      $event.returnValue = true;
    }
  }


  constructor(private route: ActivatedRoute, private alertify : AlertifyService,
    private userService : UserService, private authService : AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data=>{
      this.user= data['user'];
    })
  }

  updateProfile (){
    console.log (this.authService.decodedToken);
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(res =>{
      this.alertify.success("profile has been updated");
      this.editForm.reset(this.user);
    }, error =>{
      this.alertify.error (error);
    });

  }

}
