// filepath: /D:/Datn/Fe/admin_fe/react-admin-dashboard/i18n.js
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

// Import các file ngôn ngữ
import translationEN from './locales/en/translation.json';
import translationVI from './locales/vi/translation.json';

const resources = {
  en: {
    translation: translationEN
  },
  vi: {
    translation: translationVI
  }
};

i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: 'vi', // Ngôn ngữ mặc định
    fallbackLng: 'en',
    interpolation: {
      escapeValue: false
    }
  });

export default i18n;