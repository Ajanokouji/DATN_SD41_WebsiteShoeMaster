export const formatVietnamTime = (
  utcTimeString: string,
  includeSeconds = false,
  includeMilliseconds = false
): string => {
  // Tạo đối tượng Date từ chuỗi thời gian UTC
  const date = new Date(utcTimeString);
  
  // Kiểm tra nếu đầu vào không hợp lệ
  if (isNaN(date.getTime())) {
    return 'Thời gian không hợp lệ';
  }

  // Tính toán thời gian theo múi giờ Việt Nam (+7)
  const vietnamTime = new Date(date.getTime() + 7 * 60 * 60 * 1000);
  
  // Format ngày giờ
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  };
  
  // Thêm giây nếu được yêu cầu
  if (includeSeconds) {
    options.second = '2-digit';
  }
  
  // Format ngày giờ
  let formattedTime = vietnamTime.toLocaleString('vi-VN', options);
  
  // Thêm mili giây nếu được yêu cầu
  if (includeMilliseconds) {
    const milliseconds = vietnamTime.getMilliseconds().toString().padStart(3, '0');
    formattedTime += ',' + milliseconds;
  }
  
  return formattedTime;
};