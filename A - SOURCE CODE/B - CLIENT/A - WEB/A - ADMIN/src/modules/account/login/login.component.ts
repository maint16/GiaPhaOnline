import {Component, Inject} from '@angular/core';
import {LoginViewModel} from '../../../view-models/login.view-model';
import {IAuthenticationService} from '../../../interfaces/services/authentication-service.interface';
import {Router} from '@angular/router';
import {
  AuthService,
  GoogleLoginProvider, SocialUser
} from 'angular5-social-login';
import {AccountService} from '../../../services/account.service';
import {TranslateService} from '@ngx-translate/core';
import {TokenViewModel} from '../../../view-models/token.view-model';
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../../../constants/local-storage-key.constant';
import {GoogleLoginViewModel} from '../../../view-models/user/google-login.view-model';
import {IUserService} from '../../../interfaces/services/user-service.interface';

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
                     @Inject('IUserService') public userService: IUserService,
                     public router: Router,
                     private socialAuthService: AuthService,
                     private accountService: AccountService,
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
    this.accountService.basicLogin(this.model).subscribe((model: TokenViewModel) => {
      this.localStorageService.set(LocalStorageKeyConstant.accessToken, model.accessToken);
      console.log(model);
      // Redirect to dashboard.
      this.router.navigate(['/dashboard']);
    });
  }

  /*
  * Called when user uses social network login system.
  * */
  public ngOnFacebookLogin(): void {
    // TODO: Implement this.
    alert('Function is coming soon.');
  }

  /*
  * Called when user uses Google account to sign-in/register in system.
  * */
  public ngOnGoogleLogin(): void {
    this.socialAuthService
      .signIn(GoogleLoginProvider.PROVIDER_ID)
      .then(
        (socialUser: SocialUser) => {
          console.log(socialUser);
          let model = new GoogleLoginViewModel();
          model.idToken = socialUser.idToken;

          this.userService
            .googleLogin(model)
            .subscribe((token: TokenViewModel) => {
              console.log(token);
              this.localStorageService.set(LocalStorageKeyConstant.accessToken, token.accessToken);
              this.router.navigate(['/dashboard']);
            });
        });
  }


  //#endregion
}
