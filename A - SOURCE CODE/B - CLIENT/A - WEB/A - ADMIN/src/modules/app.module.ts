import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, NgModule} from '@angular/core';
import {AppComponent} from './app.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {FormsModule} from '@angular/forms';
import {MomentModule} from 'ngx-moment';
import {SharedModule} from './shared/shared.module';
import {AppRouteModule} from './app.route';
import {AppSettings} from '../constants/app-settings.constant';
import {ServiceModule} from '../services/service.module';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {HttpClient, HttpClientModule} from '@angular/common/http';
import {TranslateHttpLoader} from '@ngx-translate/http-loader';
import {ResolveModule} from '../resolves/resolve.module';
import {GuardModule} from '../guards/guard.module';
import {AppConfigService} from '../services/app-config.service';
import {AuthServiceConfig, FacebookLoginProvider, GoogleLoginProvider, SocialLoginModule} from 'angular5-social-login';
import {ConfigLoginConstant} from '../constants/config-login.constant';
import {AccountService} from '../services/account.service';
import {ToastrModule, ToastrService} from 'ngx-toastr';
import {NgxPaginationModule} from 'ngx-pagination';
import {ToUserStatusTitlePipe} from '../pipes/to-user-status-title.pipe';

//#region Factory functions

export function appConfigServiceFactory(appConfigService: AppConfigService, configLogin: ConfigLoginConstant) {
  return () => appConfigService.loadConfigurationFromFile();
}

//#endregion

// Configs
export function getAuthServiceConfigs() {
  let config = new AuthServiceConfig(
    [
      {
        id: FacebookLoginProvider.PROVIDER_ID,
        provider: new FacebookLoginProvider(ConfigLoginConstant.facebookAppId)
      },
      {
        id: GoogleLoginProvider.PROVIDER_ID,
        provider: new GoogleLoginProvider(ConfigLoginConstant.googleAppId)
      },
    ]
  );
  return config;
}

//#region Module declaration

@NgModule({
  declarations: [],
  imports: [
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule, // required animations module
    MomentModule,
    HttpClientModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),

    // Application modules.
    GuardModule.forRoot(),
    ServiceModule.forRoot(),
    ResolveModule.forRoot(),
    SharedModule,
    AppRouteModule,
    SocialLoginModule,
    ToastrModule.forRoot(),
    NgxPaginationModule
  ],
  providers: [
    AppSettings,
    AppConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: appConfigServiceFactory,
      multi: true,
      deps: [AppConfigService]
    },
    {
      provide: AuthServiceConfig,
      useFactory: getAuthServiceConfigs
    },
    ConfigLoginConstant,
    AccountService,
    ToastrService
  ],
  bootstrap: [AppComponent]
})


export class AppModule {
}

// required for AOT compilation
export function HttpLoaderFactory(httpClient: HttpClient) {
  return new TranslateHttpLoader(httpClient, '/src/assets/i18n/', '.json');
}

//#endregion
