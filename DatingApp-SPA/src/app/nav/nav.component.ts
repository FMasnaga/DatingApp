import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/Alertify.service';
import { AuthService } from '../_services/Auth.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model : any = {};
  photoUrl : string; 

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { 

  }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(url => this.photoUrl = url);
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    }, error =>{
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/members']);
    });
  }

  loggedIn(){
    return this.authService.loggedIn();
  }

  logOut(){
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken=null;
    this.authService.currentUser=null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
  }

}
