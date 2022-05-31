import { Device } from './device';

export interface LoginRequest {
  username: string;
  password: string;
  device: Device;
}
