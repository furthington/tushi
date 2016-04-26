(ns tushi.debug
  (:require [tushi.application :as app])
  (:import [UnityEngine Debug]))

(defn platform-log
  [msg]
  (Debug/Log msg))

(defmacro log
  [& args]
  (when app/debug?
    `(platform-log (str ~@args))))
