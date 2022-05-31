import { Device } from './device';

export interface RegisterRequest {
  username: string;
  password: string;
  device: Device;
}
