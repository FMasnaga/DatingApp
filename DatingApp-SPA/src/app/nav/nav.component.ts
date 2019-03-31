import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/Alertify.service';
import { AuthService } from '../_services/Auth.service';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model : any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService) { 

  }

  ngOnInit() {
  
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    }, error =>{
      this.alertify.error(error);
    });
  }

  loggedIn(){
    return this.authService.loggedIn();
  }

  loggedOut(){
    localStorage.removeItem('token');
    this.alertify.message('logged out');
  }

}