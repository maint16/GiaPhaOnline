import {AuthServiceConfig, FacebookLoginProvider, GoogleLoginProvider} from 'angular5-social-login';
import {AppConfigService} from '../services/app-config.service';
import {AuthServiceConfigItem} from 'angular5-social-login/auth.service';

export function externalAuthenticationFactory(appConfigService: AppConfigService) {

  // Get application configuration.
  let appConfig = appConfigService.loadAppConfig();

  // Enlist of supported external authentication providers.
  let providers: AuthServiceConfigItem[] = [];

  // Facebook provider.
  if (appConfig.facebookClientId) {
    let facebookProvider: AuthServiceConfigItem = {
      id: FacebookLoginProvider.PROVIDER_ID,
      provider: new FacebookLoginProvider(appConfig.facebookClientId)
    };
    providers.push(facebookProvider);
  }

  // Google provider.
  if (appConfig.googleClientId) {
    let googleProvider: AuthServiceConfigItem = {
      id: GoogleLoginProvider.PROVIDER_ID,
      provider: new GoogleLoginProvider(appConfig.googleClientId)
    };
    providers.push(googleProvider);
  }

  return new AuthServiceConfig(providers);
}
