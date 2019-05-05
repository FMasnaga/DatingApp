
import {Injectable} from '@angular/core';
import { User } from 'src/models/User';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/Alertify.service';
import { UserService } from '../_services/User.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/Auth.service';

@Injectable() 
export class ListResolver implements Resolve<User[]>{
    pageNumber = 1;
    pageSize = 5;
    LikesParams = 'UserLike';

    constructor(private userService : UserService, private alertify: AlertifyService, private router: Router){
    }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]>{
        return this.userService.getUsers(this.pageNumber, this.pageSize,null, this.LikesParams).pipe(
            catchError(error =>{
                this.alertify.error("problem retriving data");
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}