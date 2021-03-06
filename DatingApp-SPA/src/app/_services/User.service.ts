import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from 'src/models/User';
import { PaginatedResult } from 'src/models/Pagination';
import { map } from 'rxjs/operators';



@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  
  getUsers(page?, itemPerPage?, userParams?, likeParam?) : Observable<PaginatedResult<User[]>>{
    const paginatedResult : PaginatedResult<User[]> = new PaginatedResult<User[]>();
    
    let params = new HttpParams ();
    if (page != null && itemPerPage != null){
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }

    if(userParams != null) {
        params = params.append ('minAge', userParams.minAge);
        params = params.append ('maxAge', userParams.maxAge);
        params = params.append ('gender', userParams.gender);
        params = params.append ('orderBy', userParams.orderBy);
    }

    if (likeParam === 'UserLike'){
      params = params.append ('UserLike', 'true');
    }
    
    if (likeParam === 'LikeMe'){
      params = params.append ('LikeMe', 'true');
    }

    return this.http.get <User[]>(this.baseUrl+'users',{observe: 'response', params})
      .pipe(
        map(response =>{
          paginatedResult.result = response.body;
          if(response.headers.get('Pagination') != null){
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'))
          }
          return paginatedResult;
        })
      );
  }
  
  getUser(id) : Observable<User> {
    return this.http.get<User>(this.baseUrl+'users/'+id);
  }
  
  updateUser(id, user : User){
    return this.http.put(this.baseUrl+'users/'+ id, user);
  }
  
  setMainPhoto (userId : number, id : number){
    return this.http.post(this.baseUrl+ 'users/'+  userId+ '/photos/'+ id+ '/setMain',{});
  }

  deletePhoto (userId: number, id : number) {
    return this.http.delete(this.baseUrl+ 'users/'+ userId+ '/photos/'+ id);
  }

  sendLike (userId : number, recipientId : number){
    return this.http.post (this.baseUrl+ 'users/'+userId+ '/like/'+ recipientId,{});
  }

}
