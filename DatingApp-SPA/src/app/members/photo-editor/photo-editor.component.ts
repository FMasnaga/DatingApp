import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/Auth.service';
import { UserService } from 'src/app/_services/User.service';
import { AlertifyService } from 'src/app/_services/Alertify.service';


@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos : Photo[];
  @Output() emitPhotoUrl = new EventEmitter<string>();
  uploader:FileUploader;
  hasBaseDropZoneOver:boolean = false;
  baseUrl = environment.apiUrl;
  currentMain : Photo;

  constructor(private authService : AuthService, private userService : UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initUploader();
  }
  

  public fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  initUploader (){
    this.uploader = new FileUploader({
      url : this.baseUrl+'users/'+ this.authService.decodedToken.nameid+'/photos',
      authToken : 'Bearer '+localStorage.getItem('token'),
      isHTML5 : true,
      removeAfterUpload:true,
      allowedFileType : ['image'],
      autoUpload:false,
      maxFileSize : 10*1024*1024
    });

    this.uploader.onAfterAddingFile = (File) => {File.withCredentials = false;};

    this.uploader.onSuccessItem = (item, response,status,headers) =>{
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id : res.id ,
          url : res.url ,
          dateAdded: res.dateAdded,
          description : res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto (photo:Photo){
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(()=> {
      this.currentMain = this.photos.filter (p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      //this.emitPhotoUrl.emit(photo.url);
      this.authService.setPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto (id : number){
    this.alertify.confirm("Are you sure you want to delete this photo?", () =>{
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() =>{
        this.photos.splice(this.photos.findIndex(p => p.id == id),1);
        this.alertify.success ("Photo has been successfully deleted");
      }, error =>{
        this.alertify.error("Can't Delete the photo");
      });
    });
  }


}
