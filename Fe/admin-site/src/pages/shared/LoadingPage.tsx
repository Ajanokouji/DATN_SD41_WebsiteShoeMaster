import { motion as m } from "framer-motion";
import styles from "./LoadingScreen.module.css";

const LoadingPage = () => {
  return (
    <div className={styles.root}>
      <m.div
        initial={{
          scale: 1,
          opacity: 1,
        }}
        animate={{
          scale: [1, 0.9, 0.9, 1, 1],
          opacity: [1, 0.48, 0.48, 1, 1],
        }}
        transition={{
          duration: 1.6,
          ease: [0.42, 0, 0.58, 1],
          repeatDelay: 0.5,
          repeat: Infinity,
        }}
      >
        <div className={styles.imageContainer}>
          <img
            src="/images/logo_gold.webp"
            alt="Logo"
            width={75}
            height={40}
            sizes="(max-width: 640px) 75px, 200px"
          />
        </div>
      </m.div>

      <m.div
        className={styles.border1}
        initial={{
          scale: 1.6,
          rotate: 270,
          opacity: 0.25,
        }}
        animate={{
          scale: [1.6, 1, 1, 1.6, 1.6],
          rotate: [270, 0, 0, 270, 270],
          opacity: [0.25, 1, 1, 1, 0.25],
          borderRadius: ["25%", "25%", "50%", "50%", "25%"],
        }}
        transition={{ ease: "linear", duration: 2.8, repeat: Infinity }}
      />

      <m.div
        className={styles.border2}
        initial={{
          scale: 1,
          rotate: 0,
          opacity: 1,
        }}
        animate={{
          scale: [1, 1.2, 1.2, 1, 1],
          rotate: [0, 270, 270, 0, 0],
          opacity: [1, 0.25, 0.25, 0.25, 1],
          borderRadius: ["25%", "25%", "50%", "50%", "25%"],
        }}
        transition={{
          ease: "linear",
          duration: 2.8,
          repeat: Infinity,
        }}
      />
    </div>
  );
};

export default LoadingPage;
