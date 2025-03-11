export default interface CustomerReqDto {
  status: number;
  source: number;
  social_media: number;
  service: number[];
  full_name: string;
  gender: "Nam" | "Nữ" | "Khác";
  date_of_birth: string;
  phone_number: string;
  email: string;
  follow_up_date: string;
  follow_down_date: string;
  address: string;
  city: string;
  district: string;
  ward: string;
  detailed_info: string;
  notes: string;
  comments: Comment[];
}

export interface Comment {
  index?: number;
  title: string;
  time: string;
  status_id: number;
}

export interface StatusReqDto {
  title: string;
}

export interface ServiceReqDto {
  title: string;
}

export interface SourceReqDto {
  title: string;
}

export interface SocialReqDto {
  title: string;
}
