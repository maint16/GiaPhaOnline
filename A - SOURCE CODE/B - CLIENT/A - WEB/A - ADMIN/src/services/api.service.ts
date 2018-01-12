import {Inject, Injectable} from "@angular/core";
import {Headers, Http, Response, RequestOptions} from "@angular/http";
import {Router} from "@angular/router";
import {IApiService} from "../interfaces/services/api/api-service.interface";
import {Observable} from "rxjs/Observable";

/*
 * Service which handles hyperlink of api.
 * */
@Injectable()
export class ApiService implements IApiService{

  //#region Common properties

  // Key which is for storing access token.
  public tokenStorage: string;

  /*
  * Request options.
  * */
  private options: RequestOptions;

  //#endregion

  //#region Constructors

  // Initiate service with settings.
  public constructor(@Inject('IAuthenticationService') private authenticationService,
                     public http: Http,
                     public clientRoutingService: Router) {
  }

  //#endregion

  //#region Http methods

  /*
  * Send 'GET' to service.
  * */
  public get(baseUrl: string, url: string, parameters: any): any {

    // Build full request url.
    let fullUrl = baseUrl;

    if (url != null && url.length > 0)
      fullUrl = `${baseUrl}/${url}`;

    // Parameters are specified. Reconstruct 'em.
    if (parameters != null)
      fullUrl = `${fullUrl}?${this.encryptUrlParameters(parameters)}`;

    // Request to api to obtain list of available categories in system.
    return this.http.get(fullUrl, this.getOptions());
  }

  /*
  * Send 'POST' to service.
  * */
  public post(baseUrl: string, url: string, parameters: any, body: any): Observable<Response> {

    // Build full request url.
    let fullUrl = baseUrl;

    if (url != null && url.length > 0)
      fullUrl = `${baseUrl}/${url}`;

    // Parameters are specified. Reconstruct 'em.
    if (parameters != null)
      fullUrl = `${fullUrl}?${this.encryptUrlParameters(parameters)}`;

    // Request to api to obtain list of available categories in system.
    return this.http.post(fullUrl, body, this.getOptions());
  }

  /*
  * Send 'PUT' to service.
  * */
  public put(baseUrl: string, url: string, parameters: any, body: any): Observable<Response> {

    // Build full request url.
    let fullUrl = baseUrl;

    if (url != null && url.length > 0)
      fullUrl = `${baseUrl}/${url}`;

    // Parameters are specified. Reconstruct 'em.
    if (parameters != null)
      fullUrl = `${fullUrl}?${this.encryptUrlParameters(parameters)}`;

    // Request to api to obtain list of available categories in system.
    return this.http.put(fullUrl, body, this.getOptions());
  }

  /*
  * Send information to service using 'PUT' method.
  * */
  public delete(baseUrl: string, url: string, parameters: any, body: any): Observable<Response> {

    // Build full request url.
    let fullUrl = baseUrl;

    if (url != null && url.length > 0)
      fullUrl = `${baseUrl}/${url}`;

    // Parameters are specified. Reconstruct 'em.
    if (parameters != null)
      url = `${url}?${this.encryptUrlParameters(parameters)}`;

    // Request to api to obtain list of available categories in system.
    return this.http.delete(fullUrl, this.getOptions());
  }

  /*
  * Encrypt url parameters to prevent dangerous parameters are passed to service.
  * */
  public encryptUrlParameters(parameters: any): string {
    return Object.keys(parameters).map(key => {
      return [key, parameters[key]].map(encodeURIComponent).join("=");
    }).join("&");
  }

  //#endregion

  //#region Authorization

  // This function is for redirecting user to root page.
  public redirectToRoot(): void{
    this.clientRoutingService.navigate(['/']);
    return;
  }

  /*
  * Get http service.
  * */
  public getInstance(): Http{
    return this.http;
  }

  /*
  * Initiate http request header.
  * */
  public initOptions(value: RequestOptions): void {
    if (!value){
      let options = new RequestOptions();
      options.headers = new Headers();
      options.headers.append('Content-Type', 'application/json');
      this.options = options;
      return;
    }
    this.options = value;
  }

  /*
  * Get option which should be attached into request.
  * */
  public getOptions(): RequestOptions{
    if (!this.options){
      this.initOptions(null);
    }

    // Find authorization information attached into local storage.
    let authorization = this.authenticationService.getAuthorization();
    if (authorization){
      if (!this.options.headers.has('Authorization'))
        this.options.headers.append('Authorization', `Bearer ${authorization.code}`);
    }

    return this.options;
  }

  //#endregion
}
