import {AppConfigService} from '../services/app-config.service';

export function appConfigFactory(appConfigService: AppConfigService) {
  return () => appConfigService.loadAppConfigAsync();
}
