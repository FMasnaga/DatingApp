
import {Injectable} from '@angular/core';
import { User } from 'src/models/User';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/Alertify.service';
import { UserService } from '../_services/User.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable() 
export class MemberListResolver implements Resolve<User[]>{
    
    constructor(private userService : UserService, private alertify: AlertifyService, private router: Router){
    }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]>{
        return this.userService.getUsers().pipe(
            catchError(error =>{
                this.alertify.error("problem retriving data");
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}