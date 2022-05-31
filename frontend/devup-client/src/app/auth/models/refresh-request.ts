import { Device } from './device';

export interface RefreshRequest {
  token: string;
  refreshToken: string;
  device: Device;
}
