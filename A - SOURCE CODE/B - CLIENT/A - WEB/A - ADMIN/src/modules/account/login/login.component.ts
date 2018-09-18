import {Component, Inject, ViewChild} from '@angular/core';
import {LoginViewModel} from '../../../view-models/login.view-model';
import {IAuthenticationService} from '../../../interfaces/services/authentication-service.interface';
import {AuthorizationToken} from '../../../models/authorization-token';
import {Router} from '@angular/router';
import {
  AuthService,
  FacebookLoginProvider,
  GoogleLoginProvider
} from 'angular5-social-login';
import {ConfigLoginConstant} from '../../../constants/config-login.constant';
import {AccountService} from '../../../services/account.service';
import {TranslateService} from '@ngx-translate/core';
import {TokenViewModel} from '../../../view-models/token.view-model';
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../../../constants/local-storage-key.constant';

@Component({
  selector: 'account-login',
  templateUrl: 'login.component.html',
  styleUrls: ['login.component.css'],
})

export class LoginComponent {

  //#region Properties

  /*
  * Model for 2-way data binding.
  * */
  private model: LoginViewModel;

  /*
  * Whether component is busy or not.
  * */
  private bIsBusy: boolean;

  //#endregion

  //#region Constructor

  public constructor(@Inject('IAuthenticationService') public authenticationService: IAuthenticationService,
                     public router: Router,
                     private socialAuthService: AuthService,
                     private userService: AccountService,
                     private localStorageService: LocalStorageService,
                     private translate: TranslateService) {
    translate.setDefaultLang('en');
    this.model = new LoginViewModel();

  }

  //#endregion

  //#region Methods

  /*
  * Callback which is fired when login button is clicked.
  * */
  public ngOnBasicLogin($event): void {
    this.userService.basicLogin(this.model).subscribe((model: TokenViewModel) => {
      this.localStorageService.set(LocalStorageKeyConstant.accessToken, model.accessToken);
      console.log(model);
      // Redirect to dashboard.
      this.router.navigate(['/dashboard']);
    });
  }

  public socialSignIn(socialPlatform: string) {
    let socialPlatformProvider;
    if (socialPlatform == ConfigLoginConstant.facebook) {
      socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;
    }
    else if (socialPlatform == ConfigLoginConstant.google) {
      socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;
    }
    this.socialAuthService
      .signIn(socialPlatformProvider).then(
      (userData: any) => {
        var data: AuthorizationToken = {
          accessToken: userData.idToken,
          expire: 49517600,
          lifeTime: 34700961
        };

        // this.localStorageService.set(LocalStorageKeyConstant.accessToken, model.accessToken);
        this.router.navigate(['/dashboard']);
      });
  }

  //#endregion
}
