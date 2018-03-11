/**
 * Created by Linh Nguyen on 6/17/2017.
 */
import {XHRBackend, RequestOptions, Response, Http, RequestOptionsArgs, Headers, Request} from '@angular/http';
import {Observable} from "rxjs/Observable";
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import {Inject, Injectable} from "@angular/core";
import {IAuthenticationService} from "../interfaces/services/authentication-service.interface";
import {ToastrService} from "ngx-toastr";

@Injectable()
export class GlobalHttpInterceptor extends Http {

  //#region Constructor

  /*
   * Initiate handler service with injectors.
   * */
  public constructor(private backend: XHRBackend,
                     private defaultOptions: RequestOptions,
                     private toastr: ToastrService,
                     @Inject('IAuthenticationService') private authenticationService: IAuthenticationService) {
    super(backend, defaultOptions);
  }

  //#endregion

  //#region Methods

  /*
  * Catch request function and analyze its responses.
  * */
  public request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {

    // Get authorization token.
    let authorizationToken = this.authenticationService.getAuthorization();
    options = new RequestOptions();
    options.headers = new Headers();
    options.headers.append('Authorization', `Bearer ${authorizationToken.code}`);

    return super.request(url, options)
      .catch((x: Response) => {
      // Unauthorize response.
      if (x.status === 0 || x.status === 500)
        this.toastr.error('Có lỗi xảy ra trên máy chủ hệ thống. Xin hãy thử lại sau ít phút.', 'Thông tin hệ thống', null);
      else if (x.status == 401){ // Unauthenticated.

        // Clear authorization information.
        this.authenticationService.clearIdentity();

        // Redirect to login page.
        this.authenticationService.redirectToLogin();
      }
      else{

        // Find message responded from service api endpoint.
        let szMessage = '';

        // Find response json.
        try{
          // Find response json.
          let result = x.json();

          // Find response message.
          szMessage = result['message'];

        } catch (exception) {
          szMessage = x['statusText'];
        }

        // If message has been found, it should be displayed.
        if (szMessage) {
            this.toastr.error(szMessage, 'Thông tin hệ thống');
        }
      }

      return Observable.throw(x);
    });
  }

  //#endregion
}
