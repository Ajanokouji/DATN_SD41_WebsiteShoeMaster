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
  
  // Lấy ngày, tháng, năm
  const day = vietnamTime.getDate().toString().padStart(2, '0');
  const month = (vietnamTime.getMonth() + 1).toString().padStart(2, '0'); // Tháng bắt đầu từ 0
  const year = vietnamTime.getFullYear();

  // Lấy giờ, phút, giây
  const hours = vietnamTime.getHours().toString().padStart(2, '0');
  const minutes = vietnamTime.getMinutes().toString().padStart(2, '0');
  const seconds = vietnamTime.getSeconds().toString().padStart(2, '0');

  // Ghép chuỗi theo yêu cầu
  let formattedTime = `${day}/${month}/${year} | ${hours}:${minutes}`;

  // Thêm giây nếu cần
  if (includeSeconds) {
    formattedTime += `:${seconds}`;
  }

  // Thêm mili giây nếu cần
  if (includeMilliseconds) {
    const milliseconds = vietnamTime.getMilliseconds().toString().padStart(3, '0');
    formattedTime += `,${milliseconds}`;
  }

  return formattedTime;
};
