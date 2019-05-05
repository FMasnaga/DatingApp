import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/models/User';
import { AuthService } from 'src/app/_services/Auth.service';
import { AlertifyService } from 'src/app/_services/Alertify.service';
import { UserService } from 'src/app/_services/User.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  
  constructor(private authService: AuthService, private alertify : AlertifyService, private userService : UserService) { }

  ngOnInit() {
  }

  sendLike (id : number){
    this.userService.sendLike(this.authService.decodedToken.nameid, id).subscribe(data =>{
      this.alertify.success("you have liked "+ this.user.knownAs);
    }, error => {
      this.alertify.error(error);
    });
  }
}
