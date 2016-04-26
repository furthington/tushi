(ns tushi.debug
  (:require [tushi.application :as app])
  (:import [Debug]))

(defmacro log
  [& args]
  (when app/debug?
    (Debug/Log (apply pr-str args))))
